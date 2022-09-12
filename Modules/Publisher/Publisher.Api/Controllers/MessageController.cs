namespace Publisher.Api.Controllers;

using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Publisher.Api.Utils;
using Publisher.Application.UseCases.PublishMessage.Commands;

[Route("messages")]
public class MessageController : BaseController
{
    private readonly IMediator _mediator;

    private readonly ILogger _logger;

    public MessageController(IMediator mediator, ILogger<MessageController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PublishMessage([FromBody] PublishMessageCommand request)
    {
        _logger.LogError("Test");

        return await _mediator.Send(request)
            .Match(() => Accepted(), failure => Error(failure));
    }
}
