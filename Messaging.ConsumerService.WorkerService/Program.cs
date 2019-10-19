using Messaging.Common.Extensions;
using Messaging.ConsumerService.Application.Extensions;
using Messaging.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Messaging.ConsumerService.WorkerService
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddConsumerServiceEventHandlers();
                    services.AddMediatR();
                    services.AddKafkaEventBusSubscriber(hostContext.Configuration);
                    services.AddHostedService<ConsumerService>();
                });
    }
}
