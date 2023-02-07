using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Kafka;

public static class ServiceCollectionExtensions
{
    public static void AddKafka(
        this IServiceCollection services,
        Action<ConsumerConfig> consumerOptions,
        Action<ProducerConfig> producerOptions,
        Action<KafkaBuilder> configure = null)
    {
        var consumerConfig = new ConsumerConfig();
        consumerOptions?.Invoke(consumerConfig);
        var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

        services.AddSingleton(consumer);

        var producerConfig = new ProducerConfig();
        producerOptions?.Invoke(producerConfig);

        var producer = new ProducerBuilder<string, string>(producerConfig).Build();

        services.AddSingleton(producer);
        services.AddTransient<IEventBusProducer, KafkaEventBusProducer>();

        var kafkaBuilder = new KafkaBuilder(services);
        configure?.Invoke(kafkaBuilder);
    }
}
