namespace Messaging.PublishService.Api.Utils
{
    using CSharpFunctionalExtensions;
    using Microsoft.AspNetCore.Mvc;

    public class BaseController : Controller
    {
        protected new IActionResult Ok()
        {
            return base.Ok(Envelope.Ok());
        }

        protected IActionResult Ok<T>(T result)
        {
            return base.Ok(Envelope.Ok(result));
        }

        protected new IActionResult Accepted()
        {
            return base.Accepted(Envelope.Ok());
        }

        protected IActionResult Error(string errorMessage)
        {
            return BadRequest(Envelope.Error(errorMessage));
        }

        protected IActionResult Error(Result result)
        {
            return this.Error(result.Error);
        }

        protected IActionResult FromResult(Result result)
        {
            return result.IsSuccess ? Ok() : Error(result.Error);
        }
    }


}
