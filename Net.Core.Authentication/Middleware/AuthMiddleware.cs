using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Net.Core.Authentication.Entities;
using Net.Core.Authentication.Entities.Contexts;
using Net.Core.Authentication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Net.Core.Authentication.Auth
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Settings _appSettings;
        private IList<Claim> claims = new List<Claim>();

        public AuthMiddleware(RequestDelegate next, IOptions<Settings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IAuthService authService)
        {
            claims = new List<Claim>();
            //var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var authorizationToken = context.Request.Headers[HeaderNames.Authorization].FirstOrDefault()??string.Empty;
            var signature = context.Request.Headers["X-Signature"].FirstOrDefault();

            if (authorizationToken.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
            {
                var token = authorizationToken.Split(new char[] {' '}); 
                if (token != null)
                    await AttachBearerToContext(authService, token.ElementAtOrDefault(1));

                if (signature != null)
                    await AttachSignatureToContext(signature);
            }


            ///var principal = new GenericPrincipal(claimsIdentity, new string[] { });
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            context.User = claimsPrincipal;

            await _next(context);
        }

        private async Task AttachBearerToContext(IAuthService authService, string token)
        {
            try
            {
                //// attach user to context on successful jwt validation
                User user = await authService.FindByAccessToken(token);
                string json = JsonSerializer.Serialize(user);
                claims.Add(new Claim("user", json));
                //context.Items["User"] = user;
                //string[] roles = { };
                //var identity = new ClaimsIdentity(new[] { new Claim("user", json) });

                //var principal = new GenericPrincipal(identity, roles);
                //context.User = principal;
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }

        private async Task AttachSignatureToContext(string signature)
        {
            claims.Add(new Claim("signature", signature));
        }
    }

    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
