namespace Messaging.Infrastructure.ServiceBus
{
    using System.Text.Json;
    using System.Threading.Tasks;

    using Confluent.Kafka;
    using Messaging.Core.Application.Abstractions.ServiceBus;
    using Messaging.Core.Domain.Abstractions;

    internal class KafkaEventBusPublisher : IEventBusPublisher
    {
        private readonly IProducer<string, string> producer;

        public KafkaEventBusPublisher(IProducer<string, string> producer)
        {
            this.producer = producer;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, string topicName) where TEvent : IEvent
        {
            var data = JsonSerializer.Serialize(@event);
            var message = new Message<string, string>
            {
                Key = @event.GetType().Name,
                Value = data
            };

            await this.producer.ProduceAsync(topicName, message)
                .ConfigureAwait(false);
        }
    }
}
