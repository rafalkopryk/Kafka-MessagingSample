namespace Common.Infrastructure.ServiceBus;

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Application.CQRS;
using Common.Application.Extensions;
using Common.Application.ServiceBus;
using Common.Infrastructure;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Logging;

internal class KafkaEventBusSubscriber : IEventBusSubscriber 
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger _logger;
    private readonly IEventProvider _eventProvider;
    private readonly IMediator _mediator;

    public KafkaEventBusSubscriber(IConsumer<string, string> consumer, ILogger<KafkaEventBusSubscriber> logger,
        IEventProvider eventProvider, IMediator mediator)
    {
        _consumer = consumer;
        _logger = logger;
        _eventProvider = eventProvider;
        _mediator = mediator;
    }

    public async Task SubscribeEventAsync<TEvent>(CancellationToken cancellationToken) where TEvent : IEvent
    {
        using var consumer = _consumer;

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
            consumer.Close();
        }
    }

    private async Task ConsumeNextEvent(IConsumer<string, string> consumer, CancellationToken cancellationToken)
    {
        try
        {
            var message = consumer.Consume(cancellationToken);
            if (message.Message == null) return;

            var eventType = _eventProvider.GetByKey(message.Message.Key);
            var @event = JsonSerializer.Deserialize(message.Message.Value, eventType);
            if (@event == null) return;

            await _mediator.Publish(@event, cancellationToken);

            consumer.Commit();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error consuming message");
        }
    }
}

