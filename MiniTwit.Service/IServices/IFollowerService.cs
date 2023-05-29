using MiniTwit.Core.Responses;

namespace MiniTwit.Service.IServices;

public interface IFollowerService
{
    Task<APIResponse> FollowUserAsync(string userId, string targetUsername);
    Task<APIResponse> UnfollowUserAsync(string userId, string targetUsername);
}