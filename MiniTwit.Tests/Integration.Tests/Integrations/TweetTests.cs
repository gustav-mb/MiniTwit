namespace MiniTwit.Tests.Integration.Tests.Integrations;

public class TweetTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TweetTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
}