namespace Publisher.Application.Extensions;

using Common.Kafka;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publisher.Application.UseCases.PublishMessage.Commands;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(PublishMessageCommand));

        services.AddKafka(
            options => configuration.GetSection("EventBus").Bind(options),
            options => configuration.GetSection("EventBus").Bind(options));
    }
}

