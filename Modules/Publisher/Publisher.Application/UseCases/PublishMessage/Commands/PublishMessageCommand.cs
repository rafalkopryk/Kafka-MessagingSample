namespace Publisher.Application.UseCases.PublishMessage.Commands;

using Common.Application.CQRS;

public record PublishMessageCommand(MessageDto[] Messages)
    : ICommand;

public record MessageDto
{
    public string Author { get; init; }

    public string Content { get; init; }
}

