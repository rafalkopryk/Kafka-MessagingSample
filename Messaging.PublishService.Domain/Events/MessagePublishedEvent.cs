using Messaging.Core.Domain.Abstractions;
using System;

namespace Messaging.PublishService.Domain.Events
{
    public class MessagePublishedEvent : IEvent
    {
        public MessagePublishedEvent(string author, string content, DateTime date)
        {
            Author = author;
            Content = content;
            Date = date;
        }

        public string Author { get; }
        public string Content { get; }
        public DateTime Date { get; }
    }
}
