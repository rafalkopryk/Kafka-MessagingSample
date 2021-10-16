namespace Messaging.Core.Application.Abstractions.ServiceBus;

using Messaging.Core.Domain.Abstractions;
using System.Threading.Tasks;

public interface IEventBusPublisher
{
    Task PublishAsync<TEvent>(TEvent queue, string topicName) where TEvent : IEvent;
}
