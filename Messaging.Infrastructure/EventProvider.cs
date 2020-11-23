namespace Messaging.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Messaging.Core.Domain.Abstractions;

    internal class EventProvider : IEventProvider
    {
        private readonly IList<(string Key, Type Type)> registeredEvents = new List<(string, Type)>();

        public void RegisterEvent(params (string key, Type type)[] events)
        {
            foreach (var @event in events)
            {
                if (!typeof(IEvent).IsAssignableFrom(@event.type))
                    continue;

                if (this.registeredEvents.Any(x => x.Key == @event.key))
                    continue;

                this.registeredEvents.Add(@event);
            }
        }

        public Type GetByKey(string key)
        {
            return this.registeredEvents
                .FirstOrDefault(x => x.Key == key)
                .Type;
        }
    }
}