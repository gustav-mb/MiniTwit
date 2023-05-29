using MiniTwit.Core.DTOs;
using MiniTwit.Core.Responses;

namespace MiniTwit.Service.IServices;

public interface ITweetService
{
    Task<APIResponse> CreateTweetAsync(TweetCreateDTO tweetCreateDTO);
    Task<APIResponse<IEnumerable<TweetDTO>>> GetUsersTweetsAsync(string username, int limit = 30, CancellationToken ct = default);
    Task<APIResponse<IEnumerable<TweetDTO>>> GetAllNonFlaggedTweetsAsync(int limit = 30, CancellationToken ct = default);
    Task<APIResponse<IEnumerable<TweetDTO>>> GetUsersAndFollowedNonFlaggedTweetsAsync(string userId, int limit = 30, CancellationToken ct = default);
}