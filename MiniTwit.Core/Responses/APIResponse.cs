using Microsoft.AspNetCore.Mvc;
using MiniTwit.Core.Error;
using static MiniTwit.Core.Responses.HTTPResponse;

namespace MiniTwit.Core.Responses;

public record APIResponse
{
    public HTTPResponse HTTPResponse { get; init; }
    public APIError? APIError { get; init; }

    public APIResponse(HTTPResponse httpResponse, string error)
    {
        HTTPResponse = httpResponse;
        APIError = Errors.ToAPIError(httpResponse, error);
    }

    public APIResponse(HTTPResponse httpResponse)
    {
        HTTPResponse = httpResponse;
        APIError = null;
    }

    public ActionResult ToActionResult() => CreateActionResult();

    protected ActionResult CreateActionResult(object? model = default, string location = "") => HTTPResponse switch
    {
        Ok => new OkObjectResult(model),
        Created => new CreatedResult(location, model),
        NoContent => new NoContentResult(),
        BadRequest => new BadRequestObjectResult(APIError),
        Unauthorized => new UnauthorizedObjectResult(APIError),
        NotFound => new NotFoundObjectResult(APIError),
        Conflict => new ConflictObjectResult(APIError),
        _ => throw new NotSupportedException($"HTTPResponse {HTTPResponse} not supported!")
    };
}

public record APIResponse<T> : APIResponse
{
    public T? Model { get; init; }

    public APIResponse(HTTPResponse httpResponse, T? model, string error) : base(httpResponse, error)
    {
        Model = model;
    }

    public APIResponse(HTTPResponse httpResponse, T? model) : base(httpResponse)
    {
        Model = model;
    }

    public ActionResult<T> ToActionResult(string location) => base.CreateActionResult(Model, location);
    public new ActionResult<T> ToActionResult() => base.CreateActionResult(Model);
}