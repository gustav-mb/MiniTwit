using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MiniTwit.Core.DTOs;
using MiniTwit.Core.Entities;
using MiniTwit.Core.IRepositories;
using MiniTwit.Core.Responses;
using MiniTwit.Security.Hashing;
using MiniTwit.Security.Authentication.TokenGenerators;
using MiniTwit.Service.IServices;
using static MiniTwit.Core.Error.DBError;
using static MiniTwit.Core.Responses.HTTPResponse;

namespace MiniTwit.Service.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenRepository _tokenRepository;
    private readonly IHasher _hasher;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthenticationService(IUserRepository userRepository, ITokenRepository tokenRepository, IHasher hasher, ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _hasher = hasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<APIResponse<TokenDTO>> AuthenticateAsync(LoginDTO loginDTO)
    {
        if (string.IsNullOrEmpty(loginDTO.Username))
        {
            return new APIResponse<TokenDTO>(Unauthorized, null, USERNAME_MISSING);
        }

        if (string.IsNullOrEmpty(loginDTO.Password))
        {
            return new APIResponse<TokenDTO>(Unauthorized, null, PASSWORD_MISSING);
        }

        // Check username
        var dbResult = await _userRepository.GetByUsernameAsync(loginDTO.Username);

        if (dbResult.DBError != null)
        {
            return new APIResponse<TokenDTO>(Unauthorized, null, dbResult.DBError);
        }

        // Check password
        var validPassword = await CheckPasswordAsync(loginDTO.Password, dbResult.Model!);

        if (!validPassword)
        {
            return new APIResponse<TokenDTO>(Unauthorized, null, INVALID_PASSWORD);
        }

        // Create tokens
        var tokenResult = await GenerateTokenAsync(dbResult.Model!);

        return new APIResponse<TokenDTO>(Ok, tokenResult.Model);
    }

    public async Task<APIResponse<TokenDTO>> RefreshTokenAsync(TokenDTO tokenDTO)
    {
        var userId = _tokenGenerator.GetClaimFromAccessToken(ClaimTypes.NameIdentifier, tokenDTO.AccessToken);

        // Invalid access token
        if (userId == null)
        {
            return new APIResponse<TokenDTO>(Unauthorized, null, INVALID_TOKEN);
        }

        var expirationTime = _tokenGenerator.GetExpirationTimeFromAccessToken(tokenDTO.AccessToken);

        // Access Token has not expired yet
        if (expirationTime > DateTime.UtcNow)
        {
            return new APIResponse<TokenDTO>(Unauthorized, null, TOKEN_NOT_EXPIRED);
        }

        var refreshTokenResult = await _tokenRepository.GetAsync(tokenDTO.RefreshToken);

        // Token does not exist
        if (refreshTokenResult.Model == null)
        {
            return new APIResponse<TokenDTO>(Unauthorized, null, INVALID_TOKEN);
        }

        // Refresh token has expired
        if (refreshTokenResult.Model.ExpiryTime <= DateTime.UtcNow)
        {
            return new APIResponse<TokenDTO>(Unauthorized, null, TOKEN_EXPIRED);
        }

        // Refresh token has been invalidated
        if (refreshTokenResult.Model.Invalidated)
        {
            return new APIResponse<TokenDTO>(Unauthorized, null, TOKEN_INVALIDATED);
        }

        // Refresh token has been used
        if (refreshTokenResult.Model.Used)
        {
            return new APIResponse<TokenDTO>(Unauthorized, null, TOKEN_USED);
        }

        var jwtId = _tokenGenerator.GetClaimFromAccessToken(JwtRegisteredClaimNames.Jti, tokenDTO.AccessToken);

        // Refresh token does not belong to this access token
        if (jwtId != refreshTokenResult.Model.JwtId)
        {
            return new APIResponse<TokenDTO>(Unauthorized, null, INVALID_TOKEN);
        }

        // Fetch owner of access token
        var userResult = await _userRepository.GetByUserIdAsync(userId);

        // Invalid userId
        if (userResult.Model == null)
        {
            return new APIResponse<TokenDTO>(Unauthorized, null, userResult.DBError);
        }

        // Update refresh token to "used"
        await _tokenRepository.UpdateUsedAsync(refreshTokenResult.Model.Token, true);

        var tokenResult = await GenerateTokenAsync(userResult.Model);

        return new APIResponse<TokenDTO>(Ok, tokenResult.Model);
    }

    private async Task<DBResult<TokenDTO>> GenerateTokenAsync(User user)
    {
        var jwtId = Guid.NewGuid().ToString();
        var accessToken = _tokenGenerator.GenerateAccessToken(jwtId, user.Id, user.Username, user.Email);
        var refresh = _tokenGenerator.GenerateRefreshToken();

        var dbResult = await _tokenRepository.CreateAsync(jwtId, user.Id, refresh.RefreshToken, refresh.ExpiryTime);

        if (dbResult.DBError != null)
        {
            return new DBResult<TokenDTO>
            {
                DBError = dbResult.DBError,
                Model = null
            };
        }

        var tokenDTO = new TokenDTO
        {
            AccessToken = accessToken,
            RefreshToken = refresh.RefreshToken
        };

        // Removed old tokens
        await _tokenRepository.DeleteAllExpiredAsync();

        return new DBResult<TokenDTO>
        {
            DBError = null,
            Model = tokenDTO
        };
    }

    private async Task<bool> CheckPasswordAsync(string password, User user)
    {
        return await _hasher.VerifyHashAsync(password, user.Password, user.Salt);
    }
}