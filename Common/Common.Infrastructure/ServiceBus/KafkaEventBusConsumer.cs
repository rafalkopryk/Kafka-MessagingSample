﻿namespace Common.Infrastructure.ServiceBus;

using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Application.CQRS;
using Common.Application.Extensions;
using Common.Application.ServiceBus;
using Common.Infrastructure;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

internal class KafkaEventBusConsumer : IEventBusConsumer
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger _logger;
    private readonly IEventProvider _eventProvider;
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public KafkaEventBusConsumer(
        IConsumer<string, string> consumer,
        ILogger<KafkaEventBusConsumer> logger,
        IEventProvider eventProvider,
        IMediator mediator,
        IConfiguration configuration)
    {
        _consumer = consumer;
        _logger = logger;
        _eventProvider = eventProvider;
        _mediator = mediator;
        _configuration = configuration;
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
                await Task.Delay(TimeSpan.FromMilliseconds(50));
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

            using var activity = Diagnostics.Consumer.Start(message.Topic, message.Message);
            try
            {
                activity?.AddDefaultOpenTelemetryTags(message.Topic, message.Message);

                var eventType = _eventProvider.GetByKey(message.Message.Key);
                var @event = JsonSerializer.Deserialize(message.Message.Value, eventType);
                if (@event == null) return;

                consumer.Commit();

                await _mediator.Publish(@event, cancellationToken);

                activity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error consuming message");
        }
    }
}

