namespace MiniTwit.Server.Extensions;

public static class IConfigurationExtensions
{
    public static string? GetDatabaseName(this IConfiguration configuration)
    {
        return configuration.GetSection("DatabaseName").Value;
    }
}