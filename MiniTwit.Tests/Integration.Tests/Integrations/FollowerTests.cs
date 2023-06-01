namespace MiniTwit.Tests.Integration.Tests.Integrations;

public class FollowerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public FollowerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
}