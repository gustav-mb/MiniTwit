using MiniTwit.Core.DTOs;
using MiniTwit.Core.IRepositories;
using MiniTwit.Core.Responses;
using MiniTwit.Security.Hashing;
using MiniTwit.Service.IServices;
using static MiniTwit.Core.Error.DBError;
using static MiniTwit.Core.Responses.HTTPResponse;

namespace MiniTwit.Service.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IHasher _hasher;

    public UserService(IUserRepository repository, IHasher hasher)
    {
        _repository = repository;
        _hasher = hasher;
    }

    public async Task<APIResponse<UserDTO>> AuthenticateAsync(LoginDTO loginDTO)
    {
        var dbResult = await _repository.GetByUsernameAsync(loginDTO.Username);

        if (dbResult.DBError == INVALID_USERNAME)
        {
            return new APIResponse<UserDTO>(Unauthorized, null, dbResult.DBError);
        }

        if (string.IsNullOrEmpty(loginDTO.Password))
        {
            return new APIResponse<UserDTO>(Unauthorized, null, PASSWORD_MISSING);
        }

        var validPassword = await _hasher.VerifyHashAsync(loginDTO.Password, dbResult.Model!.Password, dbResult.Model.Salt);

        if (!validPassword)
        {
            return new APIResponse<UserDTO>(Unauthorized, null, INVALID_PASSWORD);
        }

        return new APIResponse<UserDTO>(Ok, dbResult.ConvertModelTo<UserDTO>());
    }

    public async Task<APIResponse<UserDTO>> GetUserByUsernameAsync(string username, CancellationToken ct = default)
    {
        var dbResult = await _repository.GetByUsernameAsync(username, ct);

        if (dbResult.DBError != null)
        {
            return new APIResponse<UserDTO>(NotFound, null, dbResult.DBError);
        }

        return new APIResponse<UserDTO>(Ok, dbResult.ConvertModelTo<UserDTO>());
    }

    public async Task<APIResponse> RegisterUserAsync(UserCreateDTO userCreateDTO)
    {
        var dbResult = await _repository.GetByUsernameAsync(userCreateDTO.Username);

        if (dbResult.Model != null)
        {
            return new APIResponse(Conflict, USERNAME_TAKEN);
        }

        if (string.IsNullOrEmpty(userCreateDTO.Username))
        {
            return new APIResponse(BadRequest, USERNAME_MISSING);
        }

        if (string.IsNullOrEmpty(userCreateDTO.Email) || !userCreateDTO.Email.Contains("@"))
        {
            return new APIResponse(BadRequest, EMAIL_MISSING_OR_INVALID);
        }

        if (string.IsNullOrEmpty(userCreateDTO.Password))
        {
            return new APIResponse(BadRequest, PASSWORD_MISSING);
        }

        var hashResult = await _hasher.HashAsync(userCreateDTO.Password);

        await _repository.CreateAsync(userCreateDTO.Username, userCreateDTO.Email, hashResult.Hash, hashResult.Salt);

        return new APIResponse(Created);
    }
}