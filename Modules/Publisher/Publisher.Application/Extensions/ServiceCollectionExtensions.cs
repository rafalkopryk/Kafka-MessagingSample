namespace Publisher.Application.Extensions;

using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Publisher.Application.UseCases.PublishMessage.Commands;

public static class ServiceCollectionExtensions
{
    public static void AddPublisherApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(PublishMessageCommand));

        services.AddPublisherCommandHandlers();
    }

    private static IServiceCollection AddPublisherCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<PublishMessageCommand, Result>, PublishMessageCommandHandler>();

        return services;
    }
}

