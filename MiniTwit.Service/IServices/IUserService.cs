using MiniTwit.Core.DTOs;
using MiniTwit.Core.Responses;

namespace MiniTwit.Service.IServices;

public interface IUserService
{
    Task<APIResponse<UserDTO>> AuthenticateAsync(LoginDTO loginDTO);
    Task<APIResponse> RegisterUserAsync(UserCreateDTO userCreateDTO);
    Task<APIResponse<UserDTO>> GetUserByUsernameAsync(string username, CancellationToken ct = default);
}