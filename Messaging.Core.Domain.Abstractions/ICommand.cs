using CSharpFunctionalExtensions;
using MediatR;

namespace Messaging.Core.Domain.Abstractions
{
    public interface ICommand : IRequest<Result>
    {
    }
}
