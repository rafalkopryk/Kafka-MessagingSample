using System;
using Common.Kafka;

namespace Publisher.Application.UseCases.PublishMessage;

[EventEnvelope(Topic = "messages.published.v1")]
public record MessagePublishedEvent(string Author, string Content, DateTimeOffset Date) : IEvent;
