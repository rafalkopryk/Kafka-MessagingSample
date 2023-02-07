namespace Common.Application.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string serviceName)
    {
        services.AddOpenTelemetryTracing(builder =>
        {
            builder.AddHttpClientInstrumentation(x =>
            {
                x.FilterHttpWebRequest = (filter) =>
                {
                    var elasticBulk = filter.RequestUri.AbsoluteUri.Contains("es01:9200/_bulk", StringComparison.OrdinalIgnoreCase);
                    var result = elasticBulk;
                    return !result;
                };
                x.RecordException = true;
            })
            .AddAspNetCoreInstrumentation(x =>
            {
                x.Filter = (filter) =>
                {
                    var swagger = filter.Request.Path.Value.Contains("swagger", StringComparison.OrdinalIgnoreCase);
                    var result = swagger;
                    return !result;
                };
                x.RecordException = true;
            })
            .AddSqlClientInstrumentation(x =>
            {
                x.SetDbStatementForText = true;
                x.SetDbStatementForStoredProcedure = true;
                x.RecordException = true;
            })
            .SetErrorStatusOnException()
            .AddSource("Common.Kafka")
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: serviceName, serviceVersion: "1.0.0")
                    .AddTelemetrySdk())
            .AddConsoleExporter()
            .AddOtlpExporter(configure =>
            {
                configure.Endpoint = new Uri(configuration.GetSection("otel:url").Value);
            });
        });

        services.AddOpenTelemetryMetrics(builder =>
        {
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(serviceName: serviceName, serviceVersion: "1.0.0")
                .AddTelemetrySdk());
            builder.AddMeter("Common.Kafka");
            builder.AddAspNetCoreInstrumentation();
            builder.AddHttpClientInstrumentation();
            builder.AddConsoleExporter();
            builder.AddOtlpExporter((configure, configureMetricReader) =>
            {
                configure.Endpoint = new Uri(configuration.GetSection("otel:url").Value);
                configureMetricReader.TemporalityPreference = MetricReaderTemporalityPreference.Cumulative;
                configureMetricReader.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            });
        });
    }
}

