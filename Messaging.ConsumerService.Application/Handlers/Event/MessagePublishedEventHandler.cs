﻿namespace Messaging.ConsumerService.Application.Handlers.Event
{
    using System;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Messaging.ConsumerService.Domain.Events;
    using Messaging.Core.Application.Abstractions.Handlers;

    internal class MessagePublishedEventHandler : IEventHandler<MessagePublishedEvent>
    {
        public async Task Handle(MessagePublishedEvent notification, CancellationToken cancellationToken)
        {
            var json = JsonSerializer.Serialize(notification);
            await Console.Out.WriteLineAsync("The message has been handled:")
                .ConfigureAwait(false);
            await Console.Out.WriteLineAsync(json)
                .ConfigureAwait(false);
        }
    }
}
