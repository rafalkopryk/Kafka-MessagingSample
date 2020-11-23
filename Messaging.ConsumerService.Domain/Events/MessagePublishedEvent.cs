namespace Messaging.ConsumerService.Domain.Events
{
    using System;

    using Messaging.Core.Domain.Abstractions;

    public record MessagePublishedEvent(string? Author, string? Content, DateTime? Date)
        : IEvent;
}
