namespace Publisher.Application.UseCases.PublishMessage.Commands;

using Common.Application.CQRS;

public record PublishMessageCommand(string Author, string Content)
    : ICommand;

