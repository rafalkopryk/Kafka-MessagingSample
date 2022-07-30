namespace Common.Infrastructure;

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Application.CQRS;
using Common.Application.Extensions;

internal class EventProvider : IEventProvider
{
    private readonly IList<(string Key, Type Type)> _registeredEvents = new List<(string, Type)>();

    public void RegisterEvent(params Type[] events)
    {
        foreach (var @event in events)
        {
            if (!typeof(IEvent).IsAssignableFrom(@event))
                continue;

            var key = @event.GetEventEnvelopeAttribute()?.Topic ?? throw new ArgumentNullException(nameof(EventEnvelopeAttribute));
            if (_registeredEvents.Any(x => x.Key == key))
                continue;

            _registeredEvents.Add((key, @event));
        }
    }

    public Type GetByKey(string key)
    {
        return _registeredEvents
            .FirstOrDefault(x => x.Key == key)
            .Type;
    }
}
