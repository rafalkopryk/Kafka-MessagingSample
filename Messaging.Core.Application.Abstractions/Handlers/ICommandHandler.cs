namespace Messaging.Core.Application.Abstractions.Handlers
{
    using CSharpFunctionalExtensions;
    using MediatR;
    using Messaging.Core.Domain.Abstractions;

    public interface ICommandHandler<in T> : IRequestHandler<T, Result>
        where T : ICommand
    {
    }
}
