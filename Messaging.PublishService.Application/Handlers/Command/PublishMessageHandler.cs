namespace Messaging.PublishService.Application.Handlers.Command;

using System;
using System.Threading;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;
using Messaging.Common.Const;
using Messaging.Core.Application.Abstractions.Handlers;
using Messaging.Core.Application.Abstractions.ServiceBus;
using Messaging.PublishService.Domain.Commands;
using Messaging.PublishService.Domain.Events;

internal class PublishMessageHandler : ICommandHandler<PublishMessageCommand>
{
    private readonly IEventBusPublisher busPublisher;

    public PublishMessageHandler(IEventBusPublisher busPublisher)
    {
        this.busPublisher = busPublisher;
    }

    public async Task<Result> Handle(PublishMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var messagePublishedEvent = new MessagePublishedEvent(request.Author, request.Content, DateTime.UtcNow);
            await this.busPublisher.PublishAsync(messagePublishedEvent, Topics.PublishedMessage);

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.ToString());
        }
    }
}

