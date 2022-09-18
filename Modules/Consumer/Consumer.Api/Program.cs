using Common.Infrastructure.Extensions;
using Common.Infrastructure.ServiceBus;
using Consumer.Application.Extensions;
using Consumer.WorkerService;
using Elastic.Apm.AspNetCore;
using Elastic.Apm.SerilogEnricher;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog((context, configuration) => configuration
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .Enrich.WithElasticApmCorrelationInfo()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["Elasticsearch:Url"]))
    {
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
    builder.AddHttpClientInstrumentation();
    builder.AddAspNetCoreInstrumentation();
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
    builder.IncludeFormattedMessage = true;
    builder.IncludeScopes = true;
    builder.ParseStateValues = true;
    builder.AddConsoleExporter();
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