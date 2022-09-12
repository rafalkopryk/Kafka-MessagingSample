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
    private readonly IEventBusConsumer _busConsumer;

    public ConsumerService(ILogger<ConsumerService> logger, IEventBusConsumer busSubscriber)
    {
        this._logger = logger;
        this._busConsumer = busSubscriber;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await _busConsumer.SubscribeEventAsync<MessagePublishedEvent>(stoppingToken);
        }
    }
}

