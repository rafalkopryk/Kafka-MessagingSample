using MediatR;
using Messaging.Api.PublishService.Api.Dtos;
using Messaging.PublishService.Api.Utils;
using Messaging.PublishService.Domain.Commands;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Messaging.PublishService.Api.Controllers
{
    [Route("api/[controller]")]
    public class MessageController : BaseController
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> PublishMessage([FromBody]PublishMessageDto request)
        {
            var result = await _mediator.Send(new PublishMessageCommand(request?.Author, request.Content))
                .ConfigureAwait(false);
            
            return FromResult(result);
        }
    }
}
