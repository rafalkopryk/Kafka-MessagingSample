using Messaging.Core.Domain.Abstractions;
using System.Threading.Tasks;

namespace Messaging.Core.Application.Abstractions.ServiceBus
{
    public interface IEventBusPublisher
    {
        Task PublishAsync<TEvent>(TEvent queue, string topicName) where TEvent : IEvent;
    }
}
