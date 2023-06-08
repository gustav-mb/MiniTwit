using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTwit.Core.Error;
using MiniTwit.Server.Extensions;
using MiniTwit.Service;

namespace MiniTwit.Server.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class FollowerController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly ILogger<FollowerController> _logger;

    public FollowerController(IServiceManager serviceManager, ILogger<FollowerController> logger)
    {
        _serviceManager = serviceManager;
        _logger = logger;
    }

    [Authorize]
    [HttpPost("{username}/follow")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> FollowUser(string username, [FromQuery] string userId)
    {
        if (userId != HttpContext.GetUserId())
        {
            return new ForbiddenObjectResult(new APIError { Status = 403, ErrorMsg = Errors.FORBIDDEN_OPERATION });
        }

        var response = await _serviceManager.FollowerService.FollowUserAsync(userId, username);
        return response.ToActionResult();
    }

    [Authorize]
    [HttpDelete("{username}/unfollow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UnfollowUser(string username, [FromQuery] string userId)
    {
        if (userId != HttpContext.GetUserId())
        {
            return new ForbiddenObjectResult(new APIError { Status = 403, ErrorMsg = Errors.FORBIDDEN_OPERATION });
        }
        
        var response = await _serviceManager.FollowerService.UnfollowUserAsync(userId, username);
        return response.ToActionResult();
    }
}