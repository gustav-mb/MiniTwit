// using Microsoft.Extensions.Options;
// using MiniTwit.Core.Data;

// namespace MiniTwit.Server.Extensions;

// public static class MongoContextExtensions
// {
//     public static void AddMongoContext<TContextService, TContextImplementation>(this IServiceCollection services, Action<MiniTwitDBSettings> mongoContextOptions) where TContextImplementation : MiniTwitDBSettings, TContextService where TContextService : class
//     {

//         services.AddScoped<TContextService, TContextImplementation>();
//     }

//     private static void AddRequiredServices(IServiceCollection services, Action<MiniTwitDBSettings> mongoContextOptions)
//     {
//         services.Configure(mongoContextOptions);
//         services.AddSingleton(provider =>
//         {
//             var options = provider.GetRequiredService<IOptions<MiniTwitDBSettings>>();
//             // var settings = MongoClientSettings.
//             return null;
//         });
//     }
// }