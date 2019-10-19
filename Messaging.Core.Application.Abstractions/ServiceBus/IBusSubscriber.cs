using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Core.Application.Abstractions.ServiceBus
{
    public interface IEventBusSubscriber
    {
        Task SubscribeEventAsync(string queue, CancellationToken cancellationToken);
    }
}
