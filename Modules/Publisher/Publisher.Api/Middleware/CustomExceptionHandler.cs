namespace Publisher.Api.Middleware;

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Publisher.Api.Utils;

public class CustomExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public CustomExceptionHandler(RequestDelegate next, ILogger<CustomExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(httpContext, e);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, $"Internal error");

        var json = JsonSerializer.Serialize(Envelope.Error(exception.Message));
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return context.Response.WriteAsync(json);
    }
}

public static class ExceptionhandlerExtensions
{
    public static IApplicationBuilder UseCustomExceptionhandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomExceptionHandler>();
    }
}

