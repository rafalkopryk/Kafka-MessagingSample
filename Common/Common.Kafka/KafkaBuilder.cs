using Microsoft.Extensions.DependencyInjection;

namespace Common.Kafka;

public class KafkaBuilder
{
    private readonly IServiceCollection _services;

    public KafkaBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public KafkaBuilder UseHandler<T, THandler>()
        where T : IEvent
        where THandler : IEventHandler<T>
    {
        _services.AddScoped(typeof(IEventHandler<T>), typeof(THandler));
        _services.AddHostedService<KafkaWorker<T>>();
        return this;
    }
}