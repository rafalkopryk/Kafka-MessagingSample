namespace Common.Kafka;

public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    public Task Handle(TEvent notification, CancellationToken cancellationToken);
}
