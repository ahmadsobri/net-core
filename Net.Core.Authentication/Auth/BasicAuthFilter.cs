using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using Net.Core.Authentication.Enum;
using Net.Core.Authentication.Exceptions.Base;
using Net.Core.Authentication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Net.Core.Authentication.Auth
{
    public class BasicAuthFilter : IAuthorizationFilter
    {
        private readonly string _realm;
        private readonly IAuthService authService;
        private IList<Claim> claims;

        public BasicAuthFilter(string realm, IAuthService _authService)
        {
            _realm = realm;
            if (string.IsNullOrWhiteSpace(_realm))
            {
                throw new ArgumentNullException(nameof(realm), @"Please provide a non-empty realm value.");
            }
            authService = _authService;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                string authHeader = context.HttpContext.Request.Headers[HeaderNames.Authorization];
                if (authHeader != null)
                {
                    var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
                    if (authHeaderValue.Scheme.Equals(AuthenticationSchemes.Basic.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        var credentials = AuthHelper.ParseFromBase64(authHeaderValue.Parameter ?? string.Empty).Split(':', 2);
                        if (IsBasicTokenValid(context, credentials.ElementAtOrDefault(0), credentials.ElementAtOrDefault(1)))
                            return;
                    }
                }

                ReturnUnauthorizedResult(context);
            }
            catch (FormatException)
            {
                ReturnUnauthorizedResult(context);
            }
        }

        public bool IsBasicTokenValid(AuthorizationFilterContext context, string account_number, string secret_key)
        {
            bool isValid = false;
            claims = new List<Claim>();
            var result = authService.ValidateBasicToken(account_number, secret_key).GetAwaiter().GetResult();
            if (result != null)
            {
                string json = JsonSerializer.Serialize(result);
                claims.Add(new Claim("user", json));
                isValid = true;
            }
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            context.HttpContext.User = claimsPrincipal;

            return isValid;
        }

        private void ReturnUnauthorizedResult(AuthorizationFilterContext context)
        {
            // Return 401 and a basic authentication challenge (causes browser to show login dialog)
            //If you simply return header value "WWW-Authenticate" : "Basic" in the response, browser will automatically show the dialog box for credentials.
            context.HttpContext.Response.Headers[HeaderNames.WWWAuthenticate] = $"Basic realm=\"{_realm}\"";
            //context.Result = new UnauthorizedResult();
            context.Result = new JsonResult(new ResponseError()
            {
                Status = ResponseCode.Unauthorized,
                Message = "Unauthorized"
            })
            { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}
