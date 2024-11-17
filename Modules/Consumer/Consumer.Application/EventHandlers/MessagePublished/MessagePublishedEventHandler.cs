using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Kafka;
using Microsoft.Extensions.Logging;

namespace Consumer.Application.EventHandlers.MessagePublished;

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

