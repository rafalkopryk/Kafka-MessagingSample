using MediatR;
using Messaging.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Publisher.Application.Extensions;
using Scalar.AspNetCore;
using System.Threading.Tasks;
using Publisher.Application.UseCases.PublishMessage;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddApplication(builder.Configuration);

builder.Services.AddCors();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.UseSwaggerUI(x => x.SwaggerEndpoint("/openapi/v1.json", "Publisher Api"));
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

var messagesEndpoints = app.MapGroup("/messages");
messagesEndpoints.MapPost("/", async Task<IResult> (PublishMessageCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return result switch
    {
        PublishMessageCommandResult.Success => TypedResults.Accepted(string.Empty),
        PublishMessageCommandResult.Error error => TypedResults.Problem(error!.Detail, statusCode: StatusCodes.Status500InternalServerError)
    };
})
.WithName("PublishMessage");

app.Run();
