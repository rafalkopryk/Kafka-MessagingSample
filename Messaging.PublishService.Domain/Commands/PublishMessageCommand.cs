using Messaging.Core.Domain.Abstractions;

namespace Messaging.PublishService.Domain.Commands
{
    public class PublishMessageCommand : ICommand
    {
        public PublishMessageCommand(string author, string content)
        {
            Author = author;
            Content = content;
        }

        public string Author { get; }
        public string Content { get; }
    }
}
