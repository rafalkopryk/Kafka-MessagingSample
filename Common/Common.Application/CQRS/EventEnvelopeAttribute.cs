using System;

namespace Common.Application.CQRS
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventEnvelopeAttribute : Attribute
    {
        public string Topic { get; set; }
    }
}
