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
    private readonly IEventBusProducer _busProducer;

    public PublishMessageCommandHandler(IEventBusProducer busProducer)
    {
        _busProducer = busProducer;
    }

    public async Task<Result> Handle(PublishMessageCommand request, CancellationToken cancellationToken)
    {

        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = 3
        };

        await Parallel.ForEachAsync(request.Messages, parallelOptions, async (message, token) =>
        {
            await _busProducer.PublishAsync(new MessagePublishedEvent(message.Author, message.Content, DateTimeOffset.Now), cancellationToken);
        });

        return Result.Success();
    }
}

