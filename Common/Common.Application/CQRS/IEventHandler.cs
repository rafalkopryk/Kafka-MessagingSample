namespace Common.Application.CQRS;
using MediatR;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IEvent
{
}
