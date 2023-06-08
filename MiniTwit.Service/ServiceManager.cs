using MiniTwit.Core.IRepositories;
using MiniTwit.Security.Hashing;
using MiniTwit.Security.Authentication.TokenGenerators;
using MiniTwit.Service.IServices;
using MiniTwit.Service.Services;

namespace MiniTwit.Service;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IUserService> _lazyUserService;
    private readonly Lazy<IFollowerService> _lazyFollowerService;
    private readonly Lazy<ITweetService> _lazyTweetService;
    private readonly Lazy<AuthenticationService> _lazyAuthenticationService;

    public IUserService UserService => _lazyUserService.Value;
    public IFollowerService FollowerService => _lazyFollowerService.Value;
    public ITweetService TweetService => _lazyTweetService.Value;
    public IAuthenticationService AuthenticationService => _lazyAuthenticationService.Value;

    public ServiceManager(IUserRepository userRepository, IFollowerRepository followerRepository, ITweetRepository tweetRepository, ITokenRepository tokenRepository, IHasher hasher, ITokenGenerator tokenGenerator)
    {
        _lazyUserService = new Lazy<IUserService>(() => new UserService(userRepository, hasher));
        _lazyFollowerService = new Lazy<IFollowerService>(() => new FollowerService(followerRepository));
        _lazyTweetService = new Lazy<ITweetService>(() => new TweetService(tweetRepository));
        _lazyAuthenticationService = new Lazy<AuthenticationService>(() => new AuthenticationService(userRepository, tokenRepository, hasher, tokenGenerator));
    }
}