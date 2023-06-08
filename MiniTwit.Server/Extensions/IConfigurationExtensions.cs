namespace MiniTwit.Server.Extensions;

public static class IConfigurationExtensions
{
    public static string? GetJwtKey(this IConfiguration configuration)
    {
        return configuration.GetSection("JwtSettings")["Key"];
    }

    public static string? GetDatabaseName(this IConfiguration configuration)
    {
        return configuration.GetSection("DatabaseName").Value;
    }
}