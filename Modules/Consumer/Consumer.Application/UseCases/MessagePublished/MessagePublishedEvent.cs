namespace Consumer.Application.UseCases.MessagePublished;

using System;
using Common.Application.CQRS;

[EventEnvelope(Topic = "event.messaging.publisher.messagePublished.v1")]
public record MessagePublishedEvent(string Author, string Content, DateTimeOffset Date)
    : IEvent;

