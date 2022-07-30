namespace Common.Infrastructure;

using System;

public interface IEventProvider
{
    void RegisterEvent(params Type[] events);

    Type GetByKey(string key);
}

