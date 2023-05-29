using Microsoft.AspNetCore.Mvc;
using MiniTwit.Core.DTOs;
using MiniTwit.Service;

namespace MiniTwit.Server.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly ILogger<UserController> _logger;

    public UserController(IServiceManager serviceManager, ILogger<UserController> logger)
    {
        _serviceManager = serviceManager;
        _logger = logger;
    }

    [HttpPost("/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDTO>> Login([FromBody] LoginDTO loginDTO)
    {
        var response = await _serviceManager.UserService.AuthenticateAsync(loginDTO);
        return response.ToActionResult();
    }

    [HttpPost("/register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Register([FromBody] UserCreateDTO userCreateDTO)
    {
        var response = await _serviceManager.UserService.RegisterUserAsync(userCreateDTO);
        return response.ToActionResult();
    }

    [HttpPost("/logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Logout()
    {
        return await Task.FromResult(Ok());
    }
}