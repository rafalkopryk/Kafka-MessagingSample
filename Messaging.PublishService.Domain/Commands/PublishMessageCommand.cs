namespace Messaging.PublishService.Domain.Commands
{
    using Messaging.Core.Domain.Abstractions;

    public record PublishMessageCommand(string Author, string Content)
        : ICommand;
}
