namespace Messaging.Core.Domain.Abstractions;

using CSharpFunctionalExtensions;
using MediatR;

public interface ICommand : IRequest<Result>
{
}
