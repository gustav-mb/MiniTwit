using Microsoft.AspNetCore.Mvc;
using MiniTwit.Core.DTOs;

namespace MiniTwit.Server.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class TweetController : ControllerBase
{
    private readonly ILogger<TweetController> _logger;

    public TweetController(ILogger<TweetController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<TweetDTO>>> Timeline([FromQuery] string userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    [HttpGet("/public")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TweetDTO>>> PublicTimeline(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    [HttpGet("/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<TweetDTO>>> UserTimeline(string username, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    [HttpPost("/add_message")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddMessage([FromBody] TweetCreateDTO tweetCreateDTO)
    {
        throw new NotImplementedException();
    }
}