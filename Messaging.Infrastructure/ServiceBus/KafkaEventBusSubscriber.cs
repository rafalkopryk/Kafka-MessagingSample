namespace Messaging.Infrastructure.ServiceBus;

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Confluent.Kafka;
using MediatR;
using Messaging.Core.Application.Abstractions.ServiceBus;
using Microsoft.Extensions.Logging;

internal class KafkaEventBusSubscriber : IEventBusSubscriber
{
    private readonly IConsumer<string, string> consumer;
    private readonly ILogger logger;
    private readonly IEventProvider eventProvider;
    private readonly IMediator mediator;

    public KafkaEventBusSubscriber(IConsumer<string, string> consumer, ILogger<KafkaEventBusSubscriber> logger,
        IEventProvider eventProvider, IMediator mediator)
    {
        this.consumer = consumer;
        this.logger = logger;
        this.eventProvider = eventProvider;
        this.mediator = mediator;
    }

    public async Task SubscribeEventAsync(string topicName, CancellationToken cancellationToken)
    {
        using var consumer = this.consumer;
        consumer.Subscribe(topicName);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ConsumeNextEvent(consumer, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            this.logger.LogInformation($"Error consuming message: {e.Message} {e.StackTrace}");
            consumer.Close();
        }
    }

    private async Task ConsumeNextEvent(IConsumer<string, string> consumer, CancellationToken cancellationToken)
    {
        try
        {
            var message = consumer.Consume(cancellationToken);
            if (message.Message == null) return;

            var eventType = this.eventProvider.GetByKey(message.Message.Key);
            var @event = JsonSerializer.Deserialize(message.Message.Value, eventType);
            if (@event == null) return;

            await this.mediator.Publish(@event, cancellationToken);

            consumer.Commit();
        }
        catch (Exception e)
        {
            this.logger.LogInformation($"Error consuming message: {e.Message} {e.StackTrace}");
        }
    }
}

