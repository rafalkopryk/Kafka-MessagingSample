using MediatR;
using Messaging.Core.Domain.Abstractions;

namespace Messaging.Core.Application.Abstractions.Handlers
{
    public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
        where TEvent : IEvent
    {
    }
}
