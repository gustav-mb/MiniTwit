using Microsoft.AspNetCore.Mvc;
using MiniTwit.Core.DTOs;

namespace MiniTwit.Server.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class FollowerController : ControllerBase
{
    private readonly ILogger<FollowerController> _logger;

    public FollowerController(ILogger<FollowerController> logger)
    {
        _logger = logger;
    }

    [HttpPost("/{username}/follow")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> FollowUser(string username, [FromQuery] string userId)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("/{username}/unfollow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UnfollowUser(string username, [FromQuery] string userId)
    {
        throw new NotImplementedException();
    }
}