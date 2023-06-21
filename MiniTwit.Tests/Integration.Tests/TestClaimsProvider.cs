using System.Security.Claims;

namespace MiniTwit.Tests.Integration.Tests;

public class TestClaimsProvider
{
    public IList<Claim> Claims { get; }

    public TestClaimsProvider()
    {
        Claims = new List<Claim>();
    }

    public static TestClaimsProvider Default()
    {
        return new TestClaimsProvider();
    }

    public static TestClaimsProvider WithNameIdentifier(string userId)
    {
        var provider = new TestClaimsProvider();
        provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

        return provider;
    }
}