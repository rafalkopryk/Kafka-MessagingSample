using Confluent.Kafka;
using Messaging.Core.Application.Abstractions.ServiceBus;
using Messaging.Core.Domain.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;

namespace Messaging.Infrastructure.ServiceBus
{
    public class KafkaEventBusPublisher : IEventBusPublisher
    {
        private readonly IProducer<string, string> _producer;

        public KafkaEventBusPublisher(IProducer<string, string> producer)
        {
            _producer = producer;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, string topicName) where TEvent : IEvent
        {
            var data = JsonSerializer.Serialize(@event);
            var message = new Message<string, string>
            {
                Key = @event.GetType().Name,
                Value = data
            };

            await _producer.ProduceAsync(topicName, message)
                .ConfigureAwait(false);
        }
    }
}
