using Messaging.Core.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Messaging.Infrastructure
{
    public class EventProvider : IEventProvider
    {
        private readonly IList<(string Key, Type Type)> _registeredEvents = new List<(string, Type)>();

        public void RegisterEvent(params (string key, Type type)[] events)
        {
            foreach (var @event in events)
            {
                if (!typeof(IEvent).IsAssignableFrom(@event.type))
                    continue;

                if (_registeredEvents.Any(x => x.Key == @event.key))
                    continue;

                _registeredEvents.Add(@event);
            }
        }

        public Type GetByKey(string key)
        {
            return _registeredEvents
                .FirstOrDefault(x => x.Key == key)
                .Type;
        }
    }
}