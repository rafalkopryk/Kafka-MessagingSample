namespace Common.Infrastructure.ServiceBus;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Confluent.Kafka;

internal static class Diagnostics
{
    private const string ActivitySourceName = "Common.Infrastructure.ServiceBus";
    public static ActivitySource ActivitySource { get; } = new ActivitySource(ActivitySourceName);

    internal static class Producer
    {
        private const string ActivityName = ActivitySourceName + ".MessageProduced";

        internal static Activity Start<TKey, TValue>(Message<TKey, TValue> message)
        {
            return ActivitySource.StartActivity(ActivityName, ActivityKind.Producer);
        }
    }

    internal static class Consumer
    {
        private const string ActivityName = ActivitySourceName + ".MessageConsumed";

        internal static Activity Start<TKey, TValue>(Message<TKey, TValue> message)
        {
            var headers = message.Headers.ToDictionary(x => x.Key, y => Encoding.ASCII.GetString(y.GetValueBytes()));
            var traceparent = headers.FirstOrDefault(x => x.Key == "traceparent").Value;
            return string.IsNullOrWhiteSpace(traceparent)
                ? ActivitySource.StartActivity(ActivityName, ActivityKind.Consumer)
                : ActivitySource.StartActivity(ActivityName, ActivityKind.Consumer, parentId: traceparent);
        }
    }

    public static Activity AddDefaultOpenTelemetryTags<TKey, TValue>(
        this Activity activity,
        string topic,
        Message<TKey, TValue> message)
    {
        activity?.AddTag(MessagingAttributes.SYSTEM, "kafka");
        activity?.AddTag(MessagingAttributes.DESTINATION, topic);
        activity?.AddTag(MessagingAttributes.DESTINATION_KIND, "topic");

        if (message.Key != null)
            activity?.AddTag(MessagingAttributes.KAFKA_MESSAGE_KEY, message.Key);

        if (message.Value != null)
        {
            int messagePayloadBytes = Encoding.UTF8.GetByteCount(message.Value.ToString());
            activity?.AddTag(MessagingAttributes.MESSAGE_PAYLOAD_SIZE_BYTES, messagePayloadBytes.ToString());
        }

        return activity;
    }
}
