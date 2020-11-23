namespace Messaging.Infrastructure.Extensions
{
    using System;
    using System.Linq;

    using Confluent.Kafka;
    using Messaging.Core.Application.Abstractions.ServiceBus;
    using Messaging.Core.Domain.Abstractions;
    using Messaging.Infrastructure.ServiceBus;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class InfrastructureExtensions
    {
        public static void ConfigureMessagingInfrastructure (this IServiceCollection serviceCollection, IConfiguration configuration)
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
                .Select(x => ((string, Type))(x.Name, x))
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
}
