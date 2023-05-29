using MiniTwit.Core.Entities;
using MiniTwit.Core.Responses;

namespace MiniTwit.Core.IRepositories;

public interface ITweetRepository
{
    Task<DBResult> CreateAsync(string authorId, string text);
    Task<DBResult<IEnumerable<Tweet>>> GetAllByUsernameAsync(string username, int limit = 30, CancellationToken ct = default);
    Task<DBResult<IEnumerable<Tweet>>> GetAllNonFlaggedAsync(int limit = 30, CancellationToken ct = default);
    Task<DBResult<IEnumerable<Tweet>>> GetAllNonFlaggedFollowedByUserIdAsync(string userId, int limit = 30, CancellationToken ct = default);
}