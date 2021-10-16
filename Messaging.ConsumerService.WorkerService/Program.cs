using Messaging.ConsumerService.Application.Extensions;
using Messaging.ConsumerService.WorkerService;
using Messaging.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddConsumerServiceApplication();
        services.AddMessagingInfrastructure(hostContext.Configuration);
        services.AddHostedService<ConsumerService>();
    })
    .Build();

await host.RunAsync();
