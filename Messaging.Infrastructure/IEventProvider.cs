namespace Messaging.Infrastructure
{
    using System;

    public interface IEventProvider
    {
        void RegisterEvent(params (string key, Type type)[] events);

        Type GetByKey(string key);
    }
}
