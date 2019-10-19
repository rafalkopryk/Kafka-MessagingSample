using CSharpFunctionalExtensions;
using MediatR;
using Messaging.PublishService.Application.Handlers.Command;
using Messaging.PublishService.Domain.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.ConsumerService.Application.Extensions
{
    public static class PublishServiceExtensions
    {
        public static void AddPublishServiceCommandHandlers(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<PublishMessageCommand, Result>, PublishMessageHandler>();
        }
    }
}
