using Consumer.Application.EventHandlers.MessagePublished;

namespace Consumer.Application.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Common.Kafka;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKafka(
            () => configuration.GetkafkaConsumer(
                configuration.GetConnectionString("Kafka"),
                configuration.GetSection("Kafka")),
            () => configuration.GetkafkaProducer(
                configuration.GetConnectionString("Kafka"),
                configuration.GetSection("Kafka")),
        builder => builder
                .UseHandler<MessagePublishedEvent, MessagePublishedEventHandler>());
    }
}
