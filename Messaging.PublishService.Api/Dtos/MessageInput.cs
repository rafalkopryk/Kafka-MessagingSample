namespace Messaging.Api.PublishService.Api.Dtos
{
    public record MessageInput
    {
        public string Author { get; init; }

        public string Content { get; init; }
    }
}
