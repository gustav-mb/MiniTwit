using Microsoft.AspNetCore.Mvc;
using MiniTwit.Core.Error;
using static MiniTwit.Core.Responses.HTTPResponse;

namespace MiniTwit.Core.Responses;

public record APIResponse
{
    public HTTPResponse HTTPResponse { get; init; }
    public APIError? Error { get; init; }

    public ActionResult ToActionResult() => CreateActionResult();

    protected ActionResult CreateActionResult(object? model = default, string location = "") => HTTPResponse switch
    {
        Ok => new OkObjectResult(model),
        Created => new CreatedResult(location, model),
        NoContent => new NoContentResult(),
        BadRequest => new BadRequestObjectResult(Error),
        Unauthorized => new UnauthorizedObjectResult(Error),
        NotFound => new NotFoundObjectResult(Error),
        Conflict => new ConflictObjectResult(Error),
        _ => throw new NotSupportedException($"HTTPResponse {HTTPResponse} not supported!")
    };
}

public record APIResponse<T> : APIResponse
{
    public T? Model { get; init; }

    public ActionResult<T> ToActionResult(string location) => base.CreateActionResult(Model, location);
    public new ActionResult<T> ToActionResult() => base.CreateActionResult(Model);
}