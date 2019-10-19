using CSharpFunctionalExtensions;
using Messaging.Common.Const;
using Messaging.Core.Application.Abstractions.Handlers;
using Messaging.Core.Application.Abstractions.ServiceBus;
using Messaging.PublishService.Domain.Commands;
using Messaging.PublishService.Domain.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging.PublishService.Application.Handlers.Command
{
    public class PublishMessageHandler : ICommandHandler<PublishMessageCommand>
    {
        private readonly IEventBusPublisher _busPublisher;

        public PublishMessageHandler(IEventBusPublisher busPublisher)
        {
            _busPublisher = busPublisher;
        }

        public async Task<Result> Handle(PublishMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var messagePublishedEvent = new MessagePublishedEvent(request.Author, request.Content, DateTime.UtcNow);
                await _busPublisher.PublishAsync(messagePublishedEvent, Topics.PublishedMessage)
                    .ConfigureAwait(false);
                
                return Result.Success();
            }
            catch (Exception e)
            {
                return Result.Failure(e.ToString());
            }
        }
    }
}
