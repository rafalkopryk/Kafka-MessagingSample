namespace Consumer.Application.UseCases.MessagePublished;

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Application.CQRS;
using Microsoft.Extensions.Logging;

internal class MessagePublishedEventHandler : IEventHandler<MessagePublishedEvent>
{
    private readonly ILogger<MessagePublishedEventHandler> _logger;

    public MessagePublishedEventHandler(ILogger<MessagePublishedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(MessagePublishedEvent notification, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(notification);
        _logger.LogInformation("Message published: {notification}", json);
    }
}

