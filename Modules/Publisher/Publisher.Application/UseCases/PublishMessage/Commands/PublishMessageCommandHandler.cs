namespace Publisher.Application.UseCases.PublishMessage.Commands;

using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Application.CQRS;
using Common.Application.ServiceBus;
using CSharpFunctionalExtensions;
using Publisher.Application.UseCases.PublishMessage.Events;

internal class PublishMessageCommandHandler : ICommandHandler<PublishMessageCommand>
{
    private readonly IEventBusPublisher busPublisher;

    public PublishMessageCommandHandler(IEventBusPublisher busPublisher)
    {
        this.busPublisher = busPublisher;
    }

    public async Task<Result> Handle(PublishMessageCommand request, CancellationToken cancellationToken)
    {
        await busPublisher.PublishAsync(new MessagePublishedEvent(request.Author, request.Content, DateTime.UtcNow));

        return Result.Success();
    }
}

