namespace Common.Infrastructure.ServiceBus;

using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Application.CQRS;
using Common.Application.Extensions;
using Common.Application.ServiceBus;
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
        var data = JsonSerializer.Serialize(@event);
        var eventEnvelope = typeof(TEvent).GetEventEnvelopeAttribute() ?? throw new ArgumentNullException(nameof(EventEnvelopeAttribute));

        var message = new Message<string, string>
        {
            Key = eventEnvelope.Topic,
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

            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        }

        _logger.LogInformation("Kafka SEND to {Topic}", eventEnvelope.Topic);
    }
}
