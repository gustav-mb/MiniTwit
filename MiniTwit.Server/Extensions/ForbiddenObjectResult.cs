using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MiniTwit.Server.Extensions;

[DefaultStatusCode(DefaultStatusCode)]
public class ForbiddenObjectResult : ObjectResult
{
    private const int DefaultStatusCode = StatusCodes.Status403Forbidden;

    public ForbiddenObjectResult([ActionResultObjectValue] object? error) : base(error)
    {
        StatusCode = DefaultStatusCode;
    }
}