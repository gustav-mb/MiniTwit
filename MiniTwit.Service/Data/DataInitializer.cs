using MongoDB.Bson;
using MiniTwit.Core;
using MiniTwit.Security.Hashing;
using MiniTwit.Service.Data.DataCreators;

namespace MiniTwit.Service.Data;

public class DataInitializer
{
    private readonly IMiniTwitContext _context;
    private readonly IHasher _hasher;

    public DataInitializer(IMiniTwitContext context, IHasher hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    public void Seed()
    {
        if (_context.Users.CountDocuments(new BsonDocument()) != 0)
        {
            return;
        }

        var hashResult = _hasher.Hash("password");

        // Users
        var gustav = UserCreator.Create("Gustav", "gustav@minitwit.com", hashResult.Hash, hashResult.Salt);
        var simon = UserCreator.Create("Simon", "simon@minitwit.com", hashResult.Hash, hashResult.Salt);
        var nikolaj = UserCreator.Create("Nikolaj", "nikolaj@minitwit.com", hashResult.Hash, hashResult.Salt);
        var victor = UserCreator.Create("Victor", "victor@minitwit.com", hashResult.Hash, hashResult.Salt);
        
         _context.Users.InsertMany(new[] { gustav, simon, nikolaj, victor });

        // Messages
        var m1 = TweetCreator.Create(gustav.Id!, gustav.Username, "Gustav's first tweet!", DateTime.UtcNow.AddDays(-1));
        var m2 = TweetCreator.Create(gustav.Id!, gustav.Username, "Gustav's second tweet!", DateTime.UtcNow.AddDays(-0.5));
        var m3 = TweetCreator.Create(gustav.Id!, gustav.Username, "Gustav's Flagged", DateTime.UtcNow, true);
        var m4 = TweetCreator.Create(simon.Id!, simon.Username, "Simon's first tweet");
        var m5 = TweetCreator.Create(simon.Id!, simon.Username, "Simon's second tweet");
        var m6 = TweetCreator.Create(simon.Id!, simon.Username, "Simon's third tweet");
        var m7 = TweetCreator.Create(nikolaj.Id!, nikolaj.Username, "Nikolaj1");
        var m8 = TweetCreator.Create(nikolaj.Id!, nikolaj.Username, "Nikolaj2");
        var m9 = TweetCreator.Create(victor.Id!, victor.Username, "Victor1");
        var m10 =TweetCreator.Create(victor.Id!, victor.Username, "Victor2");

        _context.Tweets.InsertMany(new[] { m1, m2, m3, m4, m5, m6, m7, m8, m9, m10 });

        // Followers
        var f1 = FollowerCreator.Create(gustav.Id!, simon.Id!);
        var f2 = FollowerCreator.Create(simon.Id!, nikolaj.Id!);
        var f3 = FollowerCreator.Create(simon.Id!, victor.Id!);
        var f4 = FollowerCreator.Create(victor.Id!, gustav.Id!);

        _context.Followers.InsertMany(new[] { f1, f2, f3, f4 });
    }
}