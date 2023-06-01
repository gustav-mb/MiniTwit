namespace MiniTwit.Core.MongoDB.DependencyInjection;

public class MongoDatabaseOptions
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}