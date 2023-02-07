namespace Common.Kafka;

using Common.Application.CQRS;
using System.Threading;
using System.Threading.Tasks;

public interface IEventBusProducer
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
}
