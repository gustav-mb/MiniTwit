using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTwit.Core.DTOs;
using MiniTwit.Service;

namespace MiniTwit.Server.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IServiceManager serviceManager, ILogger<AuthenticationController> logger)
    {
        _serviceManager = serviceManager;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenDTO>> Login([FromBody] LoginDTO loginDTO)
    {
        _logger.LogInformation($"Received login request from {loginDTO.Username}");

        var response = await _serviceManager.AuthenticationService.AuthenticateAsync(loginDTO);
        return response.ToActionResult();
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenDTO>> RefreshToken(TokenDTO tokenDTO)
    {
        _logger.LogInformation($"Received refresh token request for token {tokenDTO.AccessToken}");

        var response = await _serviceManager.AuthenticationService.RefreshTokenAsync(tokenDTO);
        return response.ToActionResult();
    }
}