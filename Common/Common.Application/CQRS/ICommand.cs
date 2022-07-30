namespace Common.Application.CQRS;

using CSharpFunctionalExtensions;
using MediatR;

public interface ICommand : IRequest<Result>
{
}
