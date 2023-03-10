using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class RouteMiddleware
{
    private readonly RequestDelegate _next;
    public RouteMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Value.Equals("/"))
        {
            context.Response.ContentType = "application/json";
            var result = new
            {
                register = "/register",
                get_token = "/get-token",
                refresh_token = "/refresh-token",
                test_auth = "/test-auth",
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }
        else
        {
            await _next(context);
        }
    }
}


public static class ProductionMiddlewareExtensions
{
    public static IApplicationBuilder UseRouteMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RouteMiddleware>();
    }
}