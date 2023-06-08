using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MiniTwit.Core.MongoDB.Builders;
using MongoDB.Driver;

namespace MiniTwit.Core.MongoDB.DependencyInjection;

public static class MongoContextExtensions
{
    public static void AddMongoContext<TContextInterface, TContextImplementation>(this IServiceCollection services, Action<MongoDatabaseOptions> databaseSettings) where TContextImplementation : MongoContext, TContextInterface where TContextInterface : class
    {
        // Configure MongoDatabaseSettings
        services.Configure(databaseSettings);

        // Add IMongoDatabase singleton service
        services.AddSingleton<IMongoDatabase>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<MongoDatabaseOptions>>();
            var client = new MongoClient(options.Value.ConnectionString);

            return client.GetDatabase(options.Value.DatabaseName);
        });

        // Add MongoContextOptionsBuilder singleton service
        services.AddSingleton<IMongoContextOptionsBuilder>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new MongoContextOptionsBuilder(database);
        });

        // Add MongoContext implementation as service
        services.AddScoped<TContextInterface, TContextImplementation>();
    }
}