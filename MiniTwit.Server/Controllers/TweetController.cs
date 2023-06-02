using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTwit.Core.DTOs;
using MiniTwit.Service;

namespace MiniTwit.Server.Controllers;

[Authorize]
[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class TweetController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly ILogger<TweetController> _logger;

    public TweetController(IServiceManager serviceManager, ILogger<TweetController> logger)
    {
        _serviceManager = serviceManager;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<TweetDTO>>> Timeline([FromQuery] string userId, [FromQuery] int? limit = null,  CancellationToken ct = default)
    {
        var response = await _serviceManager.TweetService.GetUsersAndFollowedNonFlaggedTweetsAsync(userId, limit, ct);
        return response.ToActionResult();
    }

    [AllowAnonymous]
    [HttpGet("public")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TweetDTO>>> PublicTimeline([FromQuery] int? limit = null, CancellationToken ct = default)
    {
        var response = await _serviceManager.TweetService.GetAllNonFlaggedTweetsAsync(limit, ct);
        return response.ToActionResult();
    }

    [AllowAnonymous]
    [HttpGet("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<TweetDTO>>> UserTimeline(string username, [FromQuery] int? limit = null, CancellationToken ct = default)
    {
        var response = await _serviceManager.TweetService.GetUsersTweetsAsync(username, limit, ct);
        return response.ToActionResult();
    }

    [HttpPost("add_message")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddMessage([FromBody] TweetCreateDTO tweetCreateDTO)
    {
        var response = await _serviceManager.TweetService.CreateTweetAsync(tweetCreateDTO);
        return response.ToActionResult();
    }
}