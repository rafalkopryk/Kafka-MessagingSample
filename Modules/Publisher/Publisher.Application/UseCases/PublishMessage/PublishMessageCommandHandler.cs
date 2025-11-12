using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Kafka;

namespace Publisher.Application.UseCases.PublishMessage;

using static PublishMessageCommandResult;

public class PublishMessageCommandHandler
{
    private readonly IEventBusProducer _busProducer;
    private readonly TimeProvider _timeProvider;

    public PublishMessageCommandHandler(IEventBusProducer busProducer, TimeProvider timeProvider)
    {
        _busProducer = busProducer;
        _timeProvider = timeProvider;
    }

    public async Task<PublishMessageCommandResult> Handle(PublishMessageCommand request, CancellationToken cancellationToken)
    {
        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = 3
        };

        await Parallel.ForEachAsync(request.Messages, parallelOptions, async (message, token) =>
        {
            await _busProducer.PublishAsync(new MessagePublishedEvent(message.Author, message.Content, _timeProvider.GetUtcNow()), cancellationToken);
        });

        return Success.Result;
    }
}
