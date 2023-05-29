using MiniTwit.Core.Entities;
using MiniTwit.Core.Responses;

namespace MiniTwit.Core.IRepositories;

public interface IUserRepository
{
    Task<DBResult> CreateAsync(string username, string email, string password);
    Task<DBResult<User>> GetByUsernameAsync(string username, CancellationToken ct = default);
}