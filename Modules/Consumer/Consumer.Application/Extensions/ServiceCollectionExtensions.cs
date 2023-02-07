namespace Consumer.Application.Extensions;

using Consumer.Application.UseCases.MessagePublished;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Common.Kafka;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(MessagePublishedEvent));

        services.AddKafka(
            options => configuration.GetSection("EventBus").Bind(options),
            options => configuration.GetSection("EventBus").Bind(options),
            builder => builder
                .UseTopic<MessagePublishedEvent>());
    }
}
