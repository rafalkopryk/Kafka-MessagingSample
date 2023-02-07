namespace Publisher.Application.UseCases.PublishMessage.Events;

using System;
using Common.Application.CQRS;
using Common.Kafka;

[EventEnvelope(Topic = "event.messaging.publisher.messagePublished.v1")]
public record MessagePublishedEvent(string Author, string Content, DateTimeOffset Date)
    : IEvent;

