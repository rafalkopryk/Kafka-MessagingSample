namespace Messaging.ConsumerService.Application.Extensions
{
    using CSharpFunctionalExtensions;
    using MediatR;
    using Messaging.PublishService.Application.Handlers.Command;
    using Messaging.PublishService.Domain.Commands;
    using Microsoft.Extensions.DependencyInjection;

    public static class PublishServiceExtensions
    {
        public static void ConfigurePublishServiceApplication(this IServiceCollection services)
        {
            services.AddPublishServiceCommandHandlers();
        }

        private static IServiceCollection AddPublishServiceCommandHandlers(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<PublishMessageCommand, Result>, PublishMessageHandler>();

            return services;
        }
    }
}
