using MiniTwit.Service.IServices;

namespace MiniTwit.Service;

public interface IServiceManager
{
    IUserService UserService { get; }
    IFollowerService FollowerService { get; }
    ITweetService TweetService { get; }
}