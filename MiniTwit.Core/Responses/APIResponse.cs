using Microsoft.AspNetCore.Mvc;
using MiniTwit.Core.Error;
using static MiniTwit.Core.Responses.HTTPResponse;

namespace MiniTwit.Core.Responses;

public record APIResponse
{
    public HTTPResponse HTTPResponse { get; init; }
    public APIError? APIError { get; init; }

    public APIResponse(HTTPResponse httpResponse, DBError? dbError)
    {
        HTTPResponse = httpResponse;
        APIError = dbError.ToAPIError(httpResponse);
    }

    public APIResponse(HTTPResponse httpResponse) : this(httpResponse, null) { }

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

    public APIResponse(HTTPResponse httpResponse, T? model, DBError? dbError) : base(httpResponse, dbError)
    {
        Model = model;
    }

    public APIResponse(HTTPResponse httpResponse, T? model) : this(httpResponse, model, null) { }

    public ActionResult<T> ToActionResult(string location) => base.CreateActionResult(Model, location);
    public new ActionResult<T> ToActionResult() => base.CreateActionResult(Model);
}