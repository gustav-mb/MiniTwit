namespace MiniTwit.Tests.Integration.Tests.Integrations;

public class UserTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public UserTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
}