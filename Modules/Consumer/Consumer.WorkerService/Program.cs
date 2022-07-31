using Common.Infrastructure.Extensions;
using Consumer.Application.Extensions;
using Consumer.WorkerService;
using Elastic.Apm.AspNetCore;
using Elastic.Apm.SerilogEnricher;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System;

var builder = WebApplication.CreateBuilder(args);

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

    builder.Services.AddConsumerApplication();
    builder.Services.AddEventBus(builder.Configuration);
    builder.Services.AddHostedService<ConsumerService>();

var app = builder.Build();

app.UseElasticApm(app.Configuration);
app.Run();

