using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace MiniTwit.Tests.Integration.Tests;

public static class WebApplicationFactoryExtensions
{
    public static HttpClient CreateClient<T>(this WebApplicationFactory<T> factory, TestClaimsProvider claimsProvider) where T : class
    {
        var fact = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                    options.DefaultScheme = "Test";
                }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                services.AddScoped<TestClaimsProvider>(_ => claimsProvider);
            });
        });

        var client = fact.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        return client;
    }
}