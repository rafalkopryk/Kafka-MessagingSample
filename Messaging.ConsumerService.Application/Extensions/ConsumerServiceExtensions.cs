namespace Messaging.ConsumerService.Application.Extensions;

using MediatR;
using Messaging.ConsumerService.Application.Handlers.Event;
using Messaging.ConsumerService.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

public static class ConsumerServiceExtensions
{
    public static void AddConsumerServiceApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(MessagePublishedEvent));

        services.AddConsumerServiceEventHandlers();
    }

    private static IServiceCollection AddConsumerServiceEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<INotificationHandler<MessagePublishedEvent>, MessagePublishedEventHandler>();

        return services;
    }
}
