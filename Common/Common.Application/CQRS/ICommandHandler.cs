namespace Common.Application.CQRS;
using CSharpFunctionalExtensions;
using MediatR;

public interface ICommandHandler<in T> : IRequestHandler<T, Result>
    where T : ICommand
{
}
