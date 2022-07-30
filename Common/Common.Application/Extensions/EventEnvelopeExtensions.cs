using Common.Application.CQRS;
using System;
using System.Linq;
using System.Reflection;

namespace Common.Application.Extensions
{
    public static class EventEnvelopeExtensions
    {
        public static EventEnvelopeAttribute? GetEventEnvelopeAttribute(this Type input)
        {
            var result = input.GetCustomAttributes<EventEnvelopeAttribute>().FirstOrDefault();
            return result;
        }
    }
}
