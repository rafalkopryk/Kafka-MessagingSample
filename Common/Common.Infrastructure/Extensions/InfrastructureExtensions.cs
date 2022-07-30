namespace Common.Infrastructure.Extensions;

using System;
using System.Linq;
using Common.Application.CQRS;
using Common.Application.ServiceBus;
using Common.Infrastructure;
using Common.Infrastructure.ServiceBus;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class InfrastructureExtensions
{
    public static void AddEventBus(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection
            .AddKafkaEventBusSubscriber(configuration)
            .AddKafkaEventBusPublisher(configuration);
    }

    private static IServiceCollection AddKafkaEventBusSubscriber(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var events = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes().Where(x => typeof(IEvent).IsAssignableFrom(x) && !x.IsInterface))
            .ToArray();

        var eventProvider = new EventProvider();
        eventProvider.RegisterEvent(events);

        var consumerConfig = new ConsumerConfig();
        configuration.Bind("EventBus", consumerConfig);
        var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

        serviceCollection.AddSingleton(consumer);
        serviceCollection.AddSingleton<IEventProvider>(eventProvider);
        serviceCollection.AddTransient<IEventBusSubscriber, KafkaEventBusSubscriber>();

        return serviceCollection;
    }

    private static IServiceCollection AddKafkaEventBusPublisher(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var producerConfig = new ProducerConfig();

        configuration.Bind("EventBus", producerConfig);
        var producer = new ProducerBuilder<string, string>(producerConfig).Build();

        serviceCollection.AddSingleton(producer);
        serviceCollection.AddTransient<IEventBusPublisher, KafkaEventBusPublisher>();

        return serviceCollection;
    }
}
