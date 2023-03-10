using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Net.Core.Authentication.Entities;
using Net.Core.Authentication.Enum;
using Net.Core.Authentication.Exceptions;
using Net.Core.Authentication.Exceptions.Base;
using Net.Core.Authentication.Logs;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerManager _logger;

    private readonly IActionDescriptorCollectionProvider collectionProvider;
    public ExceptionMiddleware(RequestDelegate next, ILoggerManager logger, IActionDescriptorCollectionProvider _collectionProvider)
    {
        _logger = logger;
        _next = next;
        collectionProvider = _collectionProvider;
    }
    public async Task InvokeAsync(HttpContext httpContext)
    {
        Log logging = null;
        try
        {
            var path = httpContext.Request.Path.Value;
            var routes = collectionProvider.ActionDescriptors
            .Items
            .OfType<ControllerActionDescriptor>()
            .Select(ad => $"/{ad.AttributeRouteInfo.Template}").ToList();

            if (routes.Any(route => path.Equals(route, StringComparison.InvariantCultureIgnoreCase)))
            {
                int size = 1024;
                var timer = Stopwatch.StartNew();
                var url = UriHelper.GetDisplayUrl(httpContext.Request);
                var requestTime = DateTime.Now;

                //get request
                var request_body = "";
                var request_header = "";
                httpContext.Request.EnableBuffering();
                using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, false, size, true))
                {
                    request_body = await reader.ReadToEndAsync();
                    httpContext.Request.Body.Seek(0, SeekOrigin.Begin);

                    var raw_header = new StringBuilder(Environment.NewLine);
                    foreach (var header in httpContext.Request.Headers)
                    {
                        raw_header.AppendLine($"{header.Key}:{header.Value}");
                    }
                    request_header = raw_header.ToString();
                }

                //get response
                var response_body = "";
                var response_header = "";
                Stream originalBody = httpContext.Response.Body;
                try
                {
                    using (var memStream = new MemoryStream())
                    {
                        httpContext.Response.Body = memStream;

                        await _next(httpContext);

                        memStream.Seek(0, SeekOrigin.Begin);
                        response_body = new StreamReader(memStream).ReadToEnd();

                        memStream.Seek(0, SeekOrigin.Begin);
                        await memStream.CopyToAsync(originalBody);
                    }
                }
                finally
                {
                    httpContext.Response.Body = originalBody;

                    var raw_header = new StringBuilder(Environment.NewLine);
                    foreach (var header in httpContext.Response.Headers)
                    {
                        raw_header.AppendLine($"{header.Key}:{header.Value}");
                    }
                    response_header = raw_header.ToString();
                }
                timer.Stop();
                var response_code = httpContext.Response.StatusCode.ToString();
                var elapsedTime = timer.ElapsedMilliseconds;

                logging = new Log
                {
                    id = Guid.NewGuid().ToString(),
                    request_url = url,
                    request_header = request_header.ToString(),
                    request_body = request_body,
                    request_time = requestTime,
                    elapsed_time = elapsedTime,
                    response_header = response_header.ToString(),
                    response_body = response_body,
                    response_time = DateTime.Now,
                    response_code = response_code
                };
            }
            else
            {
                await _next(httpContext);
            }
        }
        catch (UnauthorizedException ex)
        {
            _logger.LogError($"unauthorized exception: {ex}");
            await HandleBaseExceptionAsync(httpContext, ex);
        }
        catch (SignatureException ex)
        {
            _logger.LogError($"signature exception: {ex}");
            await HandleBaseExceptionAsync(httpContext, ex);
        }
        catch (AppException ex)
        {
            _logger.LogError($"app exception: {ex}");
            await HandleBaseExceptionAsync(httpContext, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError($"exception : {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
        finally
        {
            //logging
            if (logging != null)
            {
                var logService = httpContext.RequestServices.GetRequiredService<ILogService>();
                await logService.save(logging);
            }
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status200OK;

        await context.Response.WriteAsync(new ResponseError()
        {
            Status = ResponseCode.Ok,
            Message = "Invalid Request"
        }.ToString());
        //return;
    }

    private async Task HandleBaseExceptionAsync(HttpContext context, IBaseException ex)
    {
        context.Response.ContentType = "application/json";
        var statusCode = ex switch
        {
            SignatureException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status200OK
        };
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(new ResponseError()
        {
            Status = ex.GetStatus(),
            Message = ex.GetMessage()
        }.ToString());
        //return;
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}
