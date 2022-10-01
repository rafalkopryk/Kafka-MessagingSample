using Common.Infrastructure.Extensions;
using Common.Infrastructure.ServiceBus;
using Consumer.Application.Extensions;
using Consumer.WorkerService;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.sedc

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

builder.Services.AddControllers();

builder.Services.AddConsumerApplication();
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddHostedService<ConsumerService>();

builder.Services.AddOpenTelemetryTracing(builder =>
{
    builder.AddHttpClientInstrumentation(x=> x.RecordException = true);
    builder.AddAspNetCoreInstrumentation(x => x.RecordException = true);
    builder.SetErrorStatusOnException();
    builder.AddSource("Common.Infrastructure.ServiceBus");
    builder.SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: "Messaging.Cusumer", serviceVersion: "1.0.0"));
    builder.AddConsoleExporter();
    builder.AddOtlpExporter(configure =>
    {
        configure.Endpoint = new Uri("http://otel:4317");
    });
});


builder.Logging.AddOpenTelemetry(builder =>
{
    builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(serviceName: "Messaging.Cusumer", serviceVersion: "1.0.0"));
    builder.IncludeFormattedMessage = true;
    builder.IncludeScopes = true;
    builder.ParseStateValues = true;
    builder.AddConsoleExporter();
    builder.AddOtlpExporter(configure =>
    {
        configure.Endpoint = new Uri("http://otel:4317");
    });
});

builder.Services.AddOpenTelemetryMetrics(builder =>
{
    builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(serviceName: "Messaging.Cusumer", serviceVersion: "1.0.0"));
    builder.AddMeter("Common.Infrastructure.ServiceBus");
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
// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseSerilogRequestLogging();

//app.UseElasticApm(app.Configuration, new KafkaEventBusDiagnosticSubscriber());

app.Run();