namespace Common.Infrastructure.ServiceBus;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Application.CQRS;
using Common.Application.Extensions;
using Common.Application.ServiceBus;
using Confluent.Kafka;

internal class KafkaEventBusPublisher : IEventBusPublisher
{
    private readonly IProducer<string, string> producer;

    public KafkaEventBusPublisher(IProducer<string, string> producer)
    {
        this.producer = producer;
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var data = JsonSerializer.Serialize(@event);

        var eventEnvelope = typeof(TEvent).GetEventEnvelopeAttribute() ?? throw new ArgumentNullException(nameof(EventEnvelopeAttribute)); 

        var message = new Message<string, string>
        {
            Key = eventEnvelope.Topic,
            Value = data
        };

        await producer.ProduceAsync(eventEnvelope.Topic, message);
    }
}
