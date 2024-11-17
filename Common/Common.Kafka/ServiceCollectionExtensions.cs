using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Kafka;

public static class ServiceCollectionExtensions
{
    public static void AddKafka(
        this IServiceCollection services,
        Func<ConsumerConfig> consumerOptions,
        Func<ProducerConfig> producerOptions,
        Action<KafkaBuilder>? configure = null)
    {
        var consumerConfig = consumerOptions();
        var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

        services.AddSingleton(consumer);

        var producerConfig = producerOptions();
        var producer = new ProducerBuilder<string, string>(producerConfig).Build();

        services.AddSingleton(producer);
        services.AddTransient<IEventBusProducer, KafkaEventBusProducer>();

        var kafkaBuilder = new KafkaBuilder(services);
        configure?.Invoke(kafkaBuilder);
    }

    public static ConsumerConfig GetkafkaConsumer(this IConfiguration configuration, string connectionString, IConfigurationSection configurationSection)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

        var section = configuration.GetSection("Kafka");
        var config = new ConsumerConfig
        {
            BootstrapServers = connectionString,
            GroupId = section.GetValue<string>("groupid"),
            EnableAutoCommit = section.GetValue<bool>("enableautocommit"),
            StatisticsIntervalMs = section.GetValue<int>("statisticsintervalms"),
            AutoOffsetReset = section.GetValue<AutoOffsetReset>("autooffsetreset"),
            EnablePartitionEof = section.GetValue<bool>("enablepartitioneof"),
        };

        return config;
    }

    public static ProducerConfig GetkafkaProducer(this IConfiguration configuration, string connectionString, IConfigurationSection configurationSection)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

        var config = new ProducerConfig
        {
            BootstrapServers = connectionString,
            Acks = Acks.All,
            StatisticsIntervalMs = configurationSection.GetValue<int>("statisticsintervalms"),
            AllowAutoCreateTopics = true,
        };

        return config;
    }
}
