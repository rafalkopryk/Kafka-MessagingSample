using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Publisher.Api.Utils;
using Publisher.Application.Extensions;
using Serilog;
using Serilog.Debugging;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog((context, configuration) => configuration
    .Enrich.FromLogContext()
    .Enrich.WithSpan()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["Elasticsearch:ConnectionString"]))
    {
        TypeName = null,
        IndexFormat = $"applogs-{context.Configuration["AppName"]}-{DateTimeOffset.Now:yyy-MM-dd}",
        AutoRegisterTemplate = true,
    })
    .ReadFrom.Configuration(context.Configuration));
SelfLog.Enable(Console.Error);

builder.Services.AddControllers();

builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddPublisherApplication();

builder.Services.AddOpenTelemetryTracing(builder => builder
    .AddHttpClientInstrumentation(x =>
    {
        x.Filter = (filter) =>
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
    .SetErrorStatusOnException()
    .AddSource("Common.Infrastructure.ServiceBus")
    .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(serviceName: "Messaging.Publisher", serviceVersion: "1.0.0"))
    .AddConsoleExporter()
    .AddOtlpExporter(configure =>
    {
        configure.Endpoint = new Uri("http://otel:4317");
    }));


builder.Services.AddOpenTelemetryMetrics(builder =>
{
    builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(serviceName: "Messaging.Publisher", serviceVersion: "1.0.0"));
    builder.AddMeter("Common.Infrastructure.ServiceBus");
    builder.AddRuntimeInstrumentation();
    builder.AddAspNetCoreInstrumentation();
    builder.AddConsoleExporter();
    builder.AddOtlpExporter(configure =>
    {
        configure.Endpoint = new Uri("http://otel:4317");
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = LogEnricherExtensions.EnrichFromRequest;
});

//app.UseElasticApm(app.Configuration, new KafkaEventBusDiagnosticSubscriber());

app.Run();
