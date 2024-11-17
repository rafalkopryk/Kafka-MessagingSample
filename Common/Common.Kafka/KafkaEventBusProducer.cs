namespace Common.Kafka;

using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

internal class KafkaEventBusProducer : IEventBusProducer
{
    private readonly IProducer<string, string> _producer;

    private readonly ILogger<KafkaEventBusProducer> _logger;

    public KafkaEventBusProducer(IProducer<string, string> producer, ILogger<KafkaEventBusProducer> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent
    {
        var data = JsonSerializer.Serialize(@event, JsonSerializerOptions.Web);
        var eventEnvelope = typeof(TEvent).GetEventEnvelopeAttribute() ?? throw new ArgumentNullException(nameof(EventEnvelopeAttribute));

        var message = new Message<string, string>
        {
            Value = data,
            Headers = new Headers
            {
                new Header("traceparent", Encoding.ASCII.GetBytes(Activity.Current.Id)),
            }
        };

        using var activity = Diagnostics.Producer.Start(eventEnvelope.Topic, message);
        try
        {
            activity?.AddDefaultOpenTelemetryTags(eventEnvelope.Topic, message);

            await _producer.ProduceAsync(eventEnvelope.Topic, message, cancellationToken);

            _logger.LogInformation("Kafka SEND to {Topic}", eventEnvelope.Topic);
        }
        catch (Exception e)
        {
            activity?.AddException(e);
            throw;
        }
    }
}
