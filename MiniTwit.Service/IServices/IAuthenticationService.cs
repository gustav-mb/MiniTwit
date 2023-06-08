using MiniTwit.Core.DTOs;
using MiniTwit.Core.Responses;

namespace MiniTwit.Service.IServices;

public interface IAuthenticationService
{
    Task<APIResponse<TokenDTO>> RefreshTokenAsync(TokenDTO tokenDTO);
    Task<APIResponse<TokenDTO>> AuthenticateAsync(LoginDTO loginDTO);
}