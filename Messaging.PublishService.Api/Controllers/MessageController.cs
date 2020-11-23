namespace Messaging.PublishService.Api.Controllers
{
    using System.Threading.Tasks;

    using MediatR;
    using Messaging.Api.PublishService.Api.Dtos;
    using Messaging.PublishService.Api.Utils;
    using Messaging.PublishService.Domain.Commands;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class MessageController : BaseController
    {
        private readonly IMediator mediator;

        public MessageController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PublishMessage([FromBody]MessageInput request)
        {
            var result = await this.mediator.Send(new PublishMessageCommand(request?.Author, request.Content))
                .ConfigureAwait(false);

            return result.IsSuccess
                ? this.Accepted()
                : this.Error(result);
        }
    }
}
