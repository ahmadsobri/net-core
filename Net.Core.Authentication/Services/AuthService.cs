using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Net.Core.Authentication.Auth;
using Net.Core.Authentication.Entities;
using Net.Core.Authentication.Entities.Contexts;
using Net.Core.Authentication.Enum;
using Net.Core.Authentication.Exceptions;
using Net.Core.Authentication.Models;
using Net.Core.Authentication.Utilities;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace Net.Core.Authentication.Services
{
    public class AuthService : JsonSerialize<User>, IAuthService
    {
        private readonly AuthContext dbContext;
        private readonly IHttpContextAccessor httpContext;
        private readonly Settings _appSettings;
        private readonly double tokenExpired;
        public AuthService(AuthContext db, IHttpContextAccessor ctx, IOptions<Settings> appSettings)
        {
            dbContext = db;
            httpContext = ctx;
            _appSettings = appSettings.Value;
            tokenExpired = _appSettings.TokenExpired;
        }

        public async Task<TokenResponse> CreateToken(User user)
        {
            var accessToken = new AccessTokenBuilder(_appSettings, user.id.ToString());
            string token = accessToken.GenerateAccessToken();
            string refresh_token = accessToken.GenerateRefreshToken();
            var access_token_expires = DateTime.Now.AddMilliseconds(tokenExpired);
            var refresh_token_expires = DateTime.Now.AddMilliseconds(tokenExpired);
            try
            {
                user.access_token = token;
                user.refresh_token = refresh_token;
                user.access_token_expired = access_token_expires;
                user.access_token_generated = DateTime.Now;
                user.refresh_token_expired = refresh_token_expires;
                user.refresh_token_generated = DateTime.Now;
                dbContext.Entry(user).State = EntityState.Added;
                if (user.id > 0)
                {
                    user.updated_at = DateTime.Now;
                    dbContext.Entry(user).State = EntityState.Modified;
                }
                await dbContext.SaveChangesAsync();
            }
            catch
            {
                throw new AppException("update/create token failed");
            }

            return new TokenResponse
            {
                access_token = token,
                token_type = TokenType.Bearer,
                expires_in = tokenExpired,
                refresh_token = refresh_token
            };
        }

        public async Task<Response> CreateUser(Register model)
        {
            try
            {
                //create user
                var user = new User
                {
                    username = model.Username,
                    password = BC.HashPassword(model.Password),
                    key_secret = KeyGen.Base64(),
                    key_signature = KeyGen.Base64(),
                    status = UserStatus.Active,
                    created_at = DateTime.Now
                };
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();

                //create account
                var ac = new Account
                {
                    account_number = KeyGen.Number(10),
                    fullname = user.username,
                    user_id = user.id,
                    email = model.Email,
                    phone_number = "",
                    created_at = DateTime.Now
                };
                dbContext.Accounts.Add(ac);
                await dbContext.SaveChangesAsync();

                return new Response
                {
                    Status = ResponseCode.Ok,
                    Message = "User created",
                };
            }
            catch
            {
                throw new AppException("User created failed");
            }
        }

        public async Task<User> FindByUserName(string username)
        {
            return dbContext.Users.FirstOrDefault(c => c.username.Equals(username));
        }

        public async Task<User> ValidateAccessToken(AccessToken request)
        {
            var user = await FindByUserName(request.username);
            if (user == null)
                throw new AppException("user not found");

            //BasicAuth Validation
            //if(!ClaimsPrincipal().key_secret.Equals(user.key_secret))
            //    throw new AppException("invalid user");

            bool isVerify = BC.Verify(request.password, user.password);
            if (!isVerify)
                throw new AppException("invalid user");

            return user;
        }

        public async Task<User> ValidateRefreshToken(RefreshToken request)
        {
            var user = ClaimsPrincipal();
            var userExst = dbContext.Users.FirstOrDefault(x => x.access_token.Equals(user.access_token.Trim()) && x.refresh_token.Equals(request.refresh_token.Trim()));
            if (userExst == null)
                throw new AppException("user not found");

            if (userExst.access_token_expired < DateTime.Now)
                throw new AppException("token expired");

            return userExst;
        }

        public async Task<User> ValidateBasicToken(string accountNumber, string key)
        {
            var user = dbContext.Accounts.
                                       Join(dbContext.Users, ac => ac.user_id, us => us.id,
                                       (account, user) => new { account, user })
                                       .Where(m => m.account.account_number.Equals(accountNumber) && m.user.key_secret.Equals(key))
                                       .Select(m => m.user).FirstOrDefault();
            return user;
        }


        public async Task<User> FindById(int id)
        {
            User user = dbContext.Users.FirstOrDefault(x => x.id.Equals(id));
            if (user == null)
                throw new AppException("user not found");
            return user;
        }

        public async Task<User> FindByAccessToken(string token)
        {
            User user = dbContext.Users.FirstOrDefault(x => x.access_token.Equals(token));
            if (user == null)
                throw new AppException("Invalid token");

            return user;
        }

        public async Task<Response> Auth(Request request)
        {
            var user = ClaimsPrincipal();
            string sign = ClaimsSignature();
            string data = string.Format("{0}:{1}", request.request_id, request.account_number);
            if(!AuthHelper.Validate(data, sign, user.key_signature))
            {
                throw new SignatureException("invalid signature");
            }

            var account = dbContext.Accounts.FirstOrDefault(x => x.user_id.Equals(user.id) && x.account_number.Equals(request.account_number));
            if (account == null)
                throw new AppException("account not found");

            return new Response
            {
                Status = ResponseCode.Ok,
                Message = "Success",
                Data = new { 
                    account_number = request.account_number,
                    full_name = request.full_name
                }
            };
        }

        private User ClaimsPrincipal()
        {
            var principal = httpContext.HttpContext.User.FindFirstValue("user");
            var user = ToObject(principal);
            return user;
        }

        private string ClaimsSignature()
        {
            var signature = httpContext.HttpContext.User.FindFirstValue("signature");
            return signature;
        }
    }
}
