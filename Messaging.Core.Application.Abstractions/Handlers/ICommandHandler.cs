using CSharpFunctionalExtensions;
using MediatR;
using Messaging.Core.Domain.Abstractions;

namespace Messaging.Core.Application.Abstractions.Handlers
{
    public interface ICommandHandler<in T> : IRequestHandler<T, Result>
        where T : ICommand
    {
    }
}
