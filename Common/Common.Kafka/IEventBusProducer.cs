namespace Common.Kafka;
using System.Threading;
using System.Threading.Tasks;

public interface IEventBusProducer
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
}
