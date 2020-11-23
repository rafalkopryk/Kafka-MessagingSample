namespace Messaging.ConsumerService.WorkerService
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Messaging.Common.Const;
    using Messaging.Core.Application.Abstractions.ServiceBus;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class ConsumerService : BackgroundService
    {
        private readonly ILogger logger;
        private readonly IEventBusSubscriber busSubscriber;

        public ConsumerService(ILogger<ConsumerService> logger, IEventBusSubscriber busSubscriber)
        {
            this.logger = logger;
            this.busSubscriber = busSubscriber;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                this.logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await this.busSubscriber.SubscribeEventAsync(Topics.PublishedMessage, stoppingToken)
                    .ConfigureAwait(false);
                await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}
