namespace Publisher.Application.Extensions;

using Common.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publisher.Application.UseCases.PublishMessage;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<PublishMessageCommandHandler>();

        services.AddKafka(
            () => configuration.GetkafkaConsumer(
                configuration.GetConnectionString("Kafka"),
                configuration.GetSection("Kafka")),
            () => configuration.GetkafkaProducer(
                configuration.GetConnectionString("Kafka"),
                configuration.GetSection("Kafka")));
    }
}

