using Confluent.Kafka;
using MediatR;
using Messaging.Core.Application.Abstractions.ServiceBus;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Infrastructure.ServiceBus
{
    public class KafkaEventBusSubscriber : IEventBusSubscriber
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

        public async Task SubscribeEventAsync(string topicName, CancellationToken cancellationToken)
        {
            using var consumer = _consumer;
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
                _logger.LogInformation($"Error consuming message: {e.Message} {e.StackTrace}");
                consumer.Close();
            }
        }

        private async Task ConsumeNextEvent(IConsumer<string, string> consumer, CancellationToken cancellationToken)
        {
            try
            {
                var message = consumer.Consume(cancellationToken);
                if (message.Message == null) return;

                var eventType = _eventProvider.GetByKey(message.Key);
                var @event = JsonSerializer.Deserialize(message?.Value, eventType);
                if (@event == null) return;

                await _mediator.Publish(@event, cancellationToken)
                    .ConfigureAwait(false);

                consumer.Commit();
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Error consuming message: {e.Message} {e.StackTrace}");
            }
        }
    }
}
