using Messaging.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka", 62799)
       .WithDataVolume("kafka")
       .WithLifetime(ContainerLifetime.Persistent)
       .WithKafkaUI(x => x.WithLifetime(ContainerLifetime.Persistent));

builder.AddProject<Projects.Consumer_Api>("messaging-consumer", configure => configure.ExcludeLaunchProfile = true)
    .WithKafkaReference(kafka, "messaging-consumer")
    .WaitFor(kafka);

builder.AddProject<Projects.Publisher_Api>("messaging-publisher", configure => configure.LaunchProfileName = "Publisher.Api")
    .WithKafkaReference(kafka, "messaging-publisher")
    .WaitFor(kafka);

builder.Build().Run();

namespace Messaging.AppHost
{
    public static class ResourceBuilderExtensions
    {
        public static IResourceBuilder<T> WithKafkaReference<T>(this IResourceBuilder<T> builder, IResourceBuilder<KafkaServerResource> source, string groupid, int? port = 9092) where T : IResourceWithEnvironment
        {
            return builder
                .WithReference(source, "Kafka")
                .WithEnvironment("Kafka__groupid", groupid)
                .WithEnvironment("Kafka__enableautocommit", "false")
                .WithEnvironment("Kafka__statisticsintervalms", "5000")
                .WithEnvironment("Kafka__autooffsetreset", "earliest")
                .WithEnvironment("Kafka__enablepartitioneof", "true");
        }
    }
}