namespace MiniTwit.Core.Data;

public class MiniTwitDBSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string UsersCollectionName { get; set; } = null!;
    public string FollowersCollectionName { get; set; } = null!;
    public string TweetsCollectionName { get; set; } = null!;
}