using Microsoft.AspNetCore.Mvc;
using MiniTwit.Core.DTOs;

namespace MiniTwit.Server.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    [HttpPost("/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDTO>> Login([FromBody] LoginDTO loginDTO)
    {
        throw new NotImplementedException();
    }

    [HttpPost("/register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Login([FromBody] UserCreateDTO userCreateDTO)
    {
        throw new NotImplementedException();
    }

    [HttpPost("/logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Logout()
    {
        throw new NotImplementedException();
    }
}