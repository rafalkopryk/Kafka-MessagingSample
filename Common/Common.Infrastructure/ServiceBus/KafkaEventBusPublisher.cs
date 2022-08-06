﻿namespace Common.Infrastructure.ServiceBus;

using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Application.CQRS;
using Common.Application.Extensions;
using Common.Application.ServiceBus;
using Confluent.Kafka;
using Elastic.Apm;
using Elastic.Apm.Api;
using Microsoft.Extensions.Logging;

internal class KafkaEventBusPublisher : IEventBusPublisher
{
    private readonly IProducer<string, string> _producer;

    private readonly ILogger<KafkaEventBusPublisher> _logger;

    public KafkaEventBusPublisher(IProducer<string, string> producer, ILogger<KafkaEventBusPublisher> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent
    {
        var data = JsonSerializer.Serialize(@event);
        var eventEnvelope = typeof(TEvent).GetEventEnvelopeAttribute() ?? throw new ArgumentNullException(nameof(EventEnvelopeAttribute));

        await Agent.Tracer.CurrentTransaction.CaptureSpan($"Kafka SEND to { eventEnvelope.Topic}", ApiConstants.TypeMessaging, async () =>
        {
            Agent.Tracer.CurrentSpan.Context.Message ??= new Message
            {
                Queue = new Queue
                {
                    Name = eventEnvelope.Topic
                }
            };

            Agent.Tracer.CurrentSpan.Context.Destination ??= new Destination
            {
                Service = new Destination.DestinationService
                {
                    Resource = "kafka"
                }
            };

            var message = new Message<string, string>
            {
                Key = eventEnvelope.Topic,
                Value = data,
                Headers = new Headers
                {
                    new Header("trace.Id", Encoding.ASCII.GetBytes(Agent.Tracer.CurrentTransaction.OutgoingDistributedTracingData.SerializeToString())),
                }
            };

            await _producer.ProduceAsync(eventEnvelope.Topic, message, cancellationToken);

            _logger.LogInformation("Publish event on {Topic}", eventEnvelope.Topic);
        }, "kafka");
    }
}
