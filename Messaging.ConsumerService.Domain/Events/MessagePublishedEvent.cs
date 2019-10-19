using Messaging.Core.Domain.Abstractions;
using System;

namespace Messaging.ConsumerService.Domain.Events
{
    public class MessagePublishedEvent : IEvent
    {
        public string? Author { get; set; }
        public string? Content { get; set; }  
        public DateTime? Date { get; set; }
    }
}
