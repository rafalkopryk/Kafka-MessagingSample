namespace Common.Infrastructure.ElasticAPM.ServiceBus;

using System;
using Elastic.Apm;
using Elastic.Apm.DiagnosticSource;

public class KafkaEventBusDiagnosticSubscriber : IDiagnosticsSubscriber
{
    public IDisposable Subscribe(IApmAgent agent)
    {
        return new KafkaEventBusElasticActivityListener(agent);
    }
}