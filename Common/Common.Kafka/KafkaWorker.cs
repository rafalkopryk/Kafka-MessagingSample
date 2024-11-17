using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace Common.Kafka;

internal class KafkaWorker<TEvent> : BackgroundService
    where TEvent : IEvent
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger _logger;

    public KafkaWorker(IServiceScopeFactory serviceScopeFactory, ILogger<KafkaWorker<TEvent>> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await SubscribeEventAsync(stoppingToken);
        }
    }

    public async Task SubscribeEventAsync(CancellationToken cancellationToken)
    {
        using var serviceScope = _serviceScopeFactory.CreateAsyncScope();
        using var consumer = serviceScope.ServiceProvider.GetRequiredService<IConsumer<string, string>>();

        var topic = typeof(TEvent).GetEventEnvelopeAttribute() ?? throw new ArgumentNullException(nameof(EventEnvelopeAttribute));
        consumer.Subscribe(topic.Topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ConsumeNextEvent(consumer, cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error consuming message");
        }
    }

    private async Task ConsumeNextEvent(IConsumer<string, string> consumer, CancellationToken cancellationToken)
    {
        try
        {
            var message = consumer.Consume(TimeSpan.FromMilliseconds(200));
            if (message is null or { IsPartitionEOF : true })
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200));
            }
            
            if (message?.Message == null) return;

            Stopwatch sw = Stopwatch.StartNew();
            using var activity = Diagnostics.Consumer.Start(message.Topic, message.Message);

            try
            {
                activity?.AddDefaultOpenTelemetryTags(message.Topic, message.Message);

                var @event = JsonSerializer.Deserialize(message.Message.Value, typeof(TEvent), JsonSerializerOptions.Web);
                if (@event == null) return;

                using var serviceScope = _serviceScopeFactory.CreateAsyncScope();
                var eventHandler = serviceScope.ServiceProvider.GetRequiredService(typeof(IEventHandler<TEvent>)) as IEventHandler<TEvent>;

                await eventHandler.Handle((TEvent)@event, cancellationToken);

                consumer.Commit();

                TagList tags = [
                    new ("topic", message.Topic),
                    new ("Status", "Positive"),
                ];

                Diagnostics.ConsumeCounter.Add(1, tags);
                Diagnostics.ConsumeHistogram.Record(sw.ElapsedMilliseconds, tags);
            }
            catch (Exception e)
            {
                TagList tags = [
                    new ("topic", message.Topic),
                    new ("Status", "Positive"),
                ];

                Diagnostics.ConsumeCounter.Add(1, tags);
                Diagnostics.ConsumeHistogram.Record(sw.ElapsedMilliseconds, tags);

                activity?.AddException(e);
                throw;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error consuming message");
        }
    }
}


