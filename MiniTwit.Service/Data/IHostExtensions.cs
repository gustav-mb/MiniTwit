using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniTwit.Core;
using MiniTwit.Security.Hashing;

namespace MiniTwit.Service.Data;

public static class IHostExtensions
{
    public static IHost SeedDatabase(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<IMiniTwitContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<IHasher>();
            var dataInitializer = new DataInitializer(context, hasher);

            dataInitializer.Seed();

            return host;
        }
    }
}