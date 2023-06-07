using MiniTwit.Core.Entities;
using MiniTwit.Core.Responses;

namespace MiniTwit.Core.IRepositories;

public interface ITokenRepository
{
    Task<DBResult> CreateAsync(string jwtId, string userId, string refreshToken, DateTime expiryTime);
    Task<DBResult<RefreshToken>> GetAsync(string refreshToken);
    Task<DBResult> DeleteAllExpiredAsync();
    Task<DBResult> UpdateUsedAsync(string userId, bool used);
}