namespace Consumer.Application.Extensions;

using Consumer.Application.UseCases.MessagePublished;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddConsumerApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(MessagePublishedEvent));

        services.AddConsumerEventHandlers();
    }

    private static IServiceCollection AddConsumerEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<INotificationHandler<MessagePublishedEvent>, MessagePublishedEventHandler>();

        return services;
    }
}
