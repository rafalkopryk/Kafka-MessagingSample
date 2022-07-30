namespace Publisher.Api.Utils;

using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
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
        return StatusCode(StatusCodes.Status500InternalServerError, Envelope.Error(errorMessage));
    }

    protected IActionResult Error(Result result)
    {
        return Error(result.Error);
    }

    protected IActionResult FromResult(Result result)
    {
        return result.IsSuccess ? Ok() : Error(result.Error);
    }
}
