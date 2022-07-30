﻿namespace Publisher.Api.Controllers;

using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Publisher.Api.Dtos;
using Publisher.Api.Utils;
using Publisher.Application.UseCases.PublishMessage.Commands;

[Route("messages")]
public class MessageController : BaseController
{
    private readonly IMediator _mediator;

    public MessageController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PublishMessage([FromBody] MessageInput request)
    {
        return await _mediator.Send(new PublishMessageCommand(request?.Author, request.Content))
            .Match(() => Accepted(), failure => Error(failure));
    }
}
