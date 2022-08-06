namespace Common.Application.ServiceBus;

using Common.Application.CQRS;
using System.Threading;
using System.Threading.Tasks;

public interface IEventBusPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
}
