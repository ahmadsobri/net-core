using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Net.Core.Authentication.Enum;
using Net.Core.Authentication.Exceptions.Base;
using System;
using System.Linq;
using System.Text.Json;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User.FindFirst("user");
        if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any() || context.ActionDescriptor.EndpointMetadata.OfType<BasicAuthAttribute>().Any())
            return;

        if (user == null)
        {
            //context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            context.Result = new JsonResult(new ResponseError()
            {
                Status = ResponseCode.Unauthorized,
                Message = "Unauthorized"
            })
            { StatusCode = StatusCodes.Status401Unauthorized };
        }
        else
        {
            //var principal = context.HttpContext.User.FindFirstValue("user");
            var claim = JsonSerializer.Deserialize<UserPrincipal>(user.Value);

            if (claim.access_token_expired < DateTime.Now)
                context.Result = new JsonResult(new ResponseError()
                {
                    Status = ResponseCode.Fail,
                    Message = "Token Expired"
                })
                { StatusCode = StatusCodes.Status400BadRequest };
        }
    }

    private class UserPrincipal
    {
        public string username { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public DateTime? access_token_expired { get; set; }
        public DateTime? refresh_token_expired { get; set; }
    }
}
