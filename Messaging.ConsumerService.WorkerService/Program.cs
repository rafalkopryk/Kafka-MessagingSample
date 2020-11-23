namespace Messaging.ConsumerService.WorkerService
{
    using Messaging.Common.Extensions;
    using Messaging.ConsumerService.Application.Extensions;
    using Messaging.Infrastructure.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

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
                    services.AddConsumerServiceApplication();
                    services.ConfigureMediatR();
                    services.AddMessagingInfrastructure(hostContext.Configuration);
                    services.AddHostedService<ConsumerService>();
                });
    }
}
