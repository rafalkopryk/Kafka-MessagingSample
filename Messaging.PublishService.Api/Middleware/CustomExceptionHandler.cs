namespace Messaging.PublishService.Api.Middleware;

using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

using Messaging.PublishService.Api.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class CustomExceptionHandler
{
    private readonly RequestDelegate next;
    private readonly ILogger logger;

    public CustomExceptionHandler(RequestDelegate next, ILogger<CustomExceptionHandler> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public void Something(object input)
    {
        if (input is not null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        ////
    }

    public void Something(string input)
    {
        if (input is "Kurwa" and not "Patryk")
        {

        }

        if (input == "Kurwa" && input != "Patryk")
        {

        }
    }


    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await this.next(httpContext);
        }
        catch (Exception e)
        {

            await this.HandleExceptionAsync(httpContext, e);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError($"Internal error {exception.Message} {exception.StackTrace}");

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

