using Common.Infrastructure.Extensions;
using Common.Infrastructure.ServiceBus;
using Consumer.Application.Extensions;
using Consumer.WorkerService;
using Elastic.Apm.AspNetCore;
using Elastic.Apm.SerilogEnricher;
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

app.UseElasticApm(app.Configuration, new KafkaEventBusDiagnosticSubscriber());

app.Run();