using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Messaging.PublishService.Api.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Messaging.PublishService.Api.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
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
                await _next(httpContext)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {

                await HandleExceptionAsync(httpContext, e)
                    .ConfigureAwait(false);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError($"Internal error {exception.Message} {exception.StackTrace}");

            var json = JsonSerializer.Serialize(Envelope.Error(exception.Message));
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(json);
        }
    }
    

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionhandlerExtensions
    {
        public static IApplicationBuilder UseCustomExceptionhandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandler>();
        }
    }
}
