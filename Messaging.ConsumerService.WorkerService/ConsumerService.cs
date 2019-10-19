using Messaging.Common.Const;
using Messaging.Core.Application.Abstractions.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging.ConsumerService.WorkerService
{
    public class ConsumerService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IEventBusSubscriber _busSubscriber;

        public ConsumerService(ILogger<ConsumerService> logger, IEventBusSubscriber busSubscriber)
        {
            _logger = logger;
            _busSubscriber = busSubscriber;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await _busSubscriber.SubscribeEventAsync(Topics.PublishedMessage, stoppingToken)
                    .ConfigureAwait(false);
                await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}
