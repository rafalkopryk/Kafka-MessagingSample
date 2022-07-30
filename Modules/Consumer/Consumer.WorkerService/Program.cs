using Common.Infrastructure.Extensions;
using Consumer.Application.Extensions;
using Consumer.WorkerService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddConsumerApplication();
        services.AddEventBus(hostContext.Configuration);
        services.AddHostedService<ConsumerService>();
    })
    .Build();

await host.RunAsync();
