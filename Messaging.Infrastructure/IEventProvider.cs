using System;

namespace Messaging.Infrastructure
{
    public interface IEventProvider
    {
        void RegisterEvent(params (string key, Type type)[] events);

        Type GetByKey(string key);
    }
}
