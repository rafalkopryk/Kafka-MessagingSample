using MediatR;

namespace Publisher.Application.UseCases.PublishMessage;

public record PublishMessageCommand(MessageDto[] Messages)
    : IRequest<PublishMessageCommandResult>;

public record MessageDto
{
    public string Author { get; init; }

    public string Content { get; init; }
}

public abstract record PublishMessageCommandResult
{
    public record Success() : PublishMessageCommandResult
    {
        public static readonly Success Result = new ();
    }

    public record Error(string Detail) : PublishMessageCommandResult;
}
