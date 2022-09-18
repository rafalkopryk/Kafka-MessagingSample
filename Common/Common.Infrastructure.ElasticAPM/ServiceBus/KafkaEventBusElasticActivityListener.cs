namespace Common.Infrastructure.ElasticAPM.ServiceBus;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using Common.Application;
using Elastic.Apm;
using Elastic.Apm.Api;

public class KafkaEventBusElasticActivityListener : IDisposable
{
    private readonly ConcurrentDictionary<string, ITransaction> _activeTransactions = new();
    private readonly ConcurrentDictionary<string, ISpan> _activeSpans = new();

    private readonly IApmAgent _agent;

    private readonly ActivityListener _listener;

    public KafkaEventBusElasticActivityListener(IApmAgent agent)
    {
        _agent = agent;

        _listener = new ActivityListener
        {
            ActivityStarted = ActivityStarted,
            ActivityStopped = ActivityStopped,
            ShouldListenTo = activitySource => activitySource.Name == "Common.Infrastructure.ServiceBus",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
        };

        ActivitySource.AddActivityListener(_listener);
    }

    private Action<Activity> ActivityStarted =>
        activity =>
        {
            if (activity.Kind == ActivityKind.Producer)
            {
                var span = _agent.Tracer.CurrentTransaction.StartSpan(activity.OperationName, ApiConstants.TypeMessaging);
                _activeSpans.TryAdd(activity.Id, span);
                return;
            }

            if (activity.Kind == ActivityKind.Consumer)
            {
                var span = _agent.Tracer.StartTransaction(activity.OperationName, ApiConstants.TypeMessaging, distributedTracingData: DistributedTracingData.TryDeserializeFromString(activity.ParentId));
                _activeTransactions.TryAdd(activity.Id, span);
                return;
            }
        };

    private Action<Activity> ActivityStopped =>
        activity =>
        {
            if (activity.Kind == ActivityKind.Producer)
            {
                ProducerActivityStopped(activity);
                return;
            }

            if (activity.Kind == ActivityKind.Consumer)
            {
                ConsumerActivityStopped(activity);
                return;
            }
        };

    private void ConsumerActivityStopped(Activity activity)
    {
        if (_activeTransactions.TryRemove(activity.Id, out var transaction))
        {
            transaction.Duration = activity.Duration.TotalMilliseconds;
            transaction.Type = ApiConstants.TypeMessaging;
            transaction.Outcome = activity.Status switch
            {
                ActivityStatusCode.Ok => Outcome.Success,
                ActivityStatusCode.Error => Outcome.Failure,
                _ => Outcome.Unknown
            };

            var system = activity.Tags?.FirstOrDefault(n => n.Key == MessagingAttributes.SYSTEM).Value;
            var destination = activity.Tags?.FirstOrDefault(n => n.Key == MessagingAttributes.DESTINATION).Value;

            transaction.Name = $"{system} RECEIVE from {destination}";

            if (!string.IsNullOrEmpty(destination))
            {
                transaction.Context.Message ??= new Message
                {
                    Queue = new Queue
                    {
                        Name = destination
                    }
                };
            }

            transaction.End();
        }
    }

    private void ProducerActivityStopped(Activity activity)
    {
        if (_activeSpans.TryRemove(activity.Id, out var span))
        {
            span.Duration = activity.Duration.TotalMilliseconds;
            span.Type = ApiConstants.TypeMessaging;
            span.Outcome = activity.Status switch
            {
                ActivityStatusCode.Ok => Outcome.Success,
                ActivityStatusCode.Error => Outcome.Failure,
                _ => Outcome.Unknown
            };

            var system = activity.Tags?.FirstOrDefault(n => n.Key == MessagingAttributes.SYSTEM).Value;
            var destination = activity.Tags?.FirstOrDefault(n => n.Key == MessagingAttributes.DESTINATION).Value;

            span.Name = $"{system} SEND to {destination}";

            if (!string.IsNullOrEmpty(system))
            {
                span.Subtype = system;
                span.Context.Destination = new Destination
                {
                    Service = new Destination.DestinationService { Resource = system }
                };

                if (!string.IsNullOrEmpty(destination))
                {
                    span.Context.Destination.Service.Resource = $"{system}/{destination}";
                    span.Context.Message ??= new Message
                    {
                        Queue = new Queue
                        {
                            Name = destination
                        }
                    };
                }
            }

            span.End();
        }
    }

    public void Dispose() => _listener.Dispose();
}
