using System;
using Common.Kafka;

namespace Consumer.Application.EventHandlers.MessagePublished;

[EventEnvelope(Topic = "messages.published.v1")]
public record MessagePublishedEvent(string Author, string Content, DateTimeOffset Date)
    : IEvent;

