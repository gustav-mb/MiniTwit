using MiniTwit.Core.Entities;
using MiniTwit.Core.Responses;

namespace MiniTwit.Core.IRepositories;

public interface IUserRepository
{
    Task<DBResult> CreateAsync(string username, string email, string password, string salt);
    Task<DBResult<User>> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<DBResult<User>> GetByUserIdAsync(string userId, CancellationToken ct = default);
}