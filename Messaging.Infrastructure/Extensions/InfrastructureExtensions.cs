using Confluent.Kafka;
using Messaging.Core.Application.Abstractions.ServiceBus;
using Messaging.Core.Domain.Abstractions;
using Messaging.Infrastructure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Messaging.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static void AddKafkaEventBusSubscriber(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var events = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(x => typeof(IEvent).IsAssignableFrom(x) && !x.IsInterface))
                .Select(x => ((string, Type))(x.Name, x))
                .ToArray();

            var eventProvider = new EventProvider();
            eventProvider.RegisterEvent(events);

            var consumerConfig = new ConsumerConfig();
            configuration.Bind("consumer", consumerConfig);
            var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

            serviceCollection.AddSingleton(consumer);
            serviceCollection.AddSingleton<IEventProvider>(eventProvider);
            serviceCollection.AddTransient<IEventBusSubscriber, KafkaEventBusSubscriber>();
        }

        public static void AddKafkaEventBusPublisher(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var producerConfig = new ProducerConfig();

            configuration.Bind("producer", producerConfig);
            var producer = new ProducerBuilder<string, string>(producerConfig).Build();

            serviceCollection.AddSingleton(producer);
            serviceCollection.AddTransient<IEventBusPublisher, KafkaEventBusPublisher>();
        }
    }
}
