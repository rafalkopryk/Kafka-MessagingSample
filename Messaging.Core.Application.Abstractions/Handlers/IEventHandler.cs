namespace Messaging.Core.Application.Abstractions.Handlers;

using MediatR;
using Messaging.Core.Domain.Abstractions;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IEvent
{
}
