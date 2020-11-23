namespace Messaging.Core.Application.Abstractions.ServiceBus
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IEventBusSubscriber
    {
        Task SubscribeEventAsync(string queue, CancellationToken cancellationToken);
    }
}
