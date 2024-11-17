namespace Publisher.Application.Extensions;

using Common.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(ServiceCollectionExtensions).Assembly));
        services.AddKafka(
            () => configuration.GetkafkaConsumer(
                configuration.GetConnectionString("Kafka"),
                configuration.GetSection("Kafka")),
            () => configuration.GetkafkaProducer(
                configuration.GetConnectionString("Kafka"),
                configuration.GetSection("Kafka")));
    }
}

