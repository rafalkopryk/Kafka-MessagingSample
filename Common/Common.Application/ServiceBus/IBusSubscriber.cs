namespace Common.Application.ServiceBus;

using Common.Application.CQRS;
using System.Threading;
using System.Threading.Tasks;

public interface IEventBusSubscriber
{
    Task SubscribeEventAsync<T>(CancellationToken cancellationToken) where T : IEvent;
}

