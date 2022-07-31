namespace Consumer.WorkerService;

using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Application.ServiceBus;
using Consumer.Application.UseCases.MessagePublished;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class ConsumerService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IEventBusSubscriber _busSubscriber;

    public ConsumerService(ILogger<ConsumerService> logger, IEventBusSubscriber busSubscriber)
    {
        this._logger = logger;
        this._busSubscriber = busSubscriber;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await _busSubscriber.SubscribeEventAsync<MessagePublishedEvent>(stoppingToken);
            await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
        }
    }
}

