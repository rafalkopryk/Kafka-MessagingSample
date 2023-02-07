using Common.Application;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Text.Json;

namespace Common.Kafka;

internal class KafkaWorker<TEvent> : BackgroundService
    where TEvent : INotification
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
            await SubscribeEventAsync<TEvent>(stoppingToken);
        }
    }

    public async Task SubscribeEventAsync<TEvent>(CancellationToken cancellationToken) where TEvent : INotification
    {
        using var serviceScope = _serviceScopeFactory.CreateAsyncScope();
        using var consumer = serviceScope.ServiceProvider.GetRequiredService<IConsumer<string, string>>();

        var topic = typeof(TEvent).GetEventEnvelopeAttribute() ?? throw new ArgumentNullException(nameof(EventEnvelopeAttribute));
        consumer.Subscribe(topic.Topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken);
                await ConsumeNextEvent(consumer, cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error consuming message");
            consumer.Close();
        }
    }

    private async Task ConsumeNextEvent(IConsumer<string, string> consumer, CancellationToken cancellationToken)
    {
        try
        {
            using var serviceScope = _serviceScopeFactory.CreateAsyncScope();
            var mediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();
            
            var message = consumer.Consume(cancellationToken);
            if (message.Message == null) return;


            Stopwatch sw = Stopwatch.StartNew();
            using var activity = Diagnostics.Consumer.Start(message.Topic, message.Message);

            try
            {
                activity?.AddDefaultOpenTelemetryTags(message.Topic, message.Message);

                var @event = JsonSerializer.Deserialize(message.Message.Value, typeof(TEvent), JsonSerializerCustomOptions.CamelCase);
                if (@event == null) return;

                await mediator.Publish(@event, cancellationToken);

                consumer.Commit();

                var tags = new[]
                {
                    ("topic", message.Topic),
                    ("Status", "Positive")
                };

                Diagnostics.ConsumeCounter.Add(tags);
                Diagnostics.ConsumeHistogram.Record(sw.ElapsedMilliseconds, tags);
            }
            catch (Exception e)
            {
                var tags = new[]
                {
                    ("topic", message.Topic),
                    ("Status", "Positive")
                };

                Diagnostics.ConsumeCounter.Add(tags);
                Diagnostics.ConsumeHistogram.Record(sw.ElapsedMilliseconds, tags);

                activity?.RecordException(e);
                throw;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error consuming message");
        }
    }
}


