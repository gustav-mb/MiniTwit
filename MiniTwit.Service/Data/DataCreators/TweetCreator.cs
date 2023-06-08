using MiniTwit.Core.Entities;

namespace MiniTwit.Service.Data.DataCreators;

public static class TweetCreator
{
    public static Tweet Create(string authorId, string text, DateTime pubDate, bool flagged = false)
    {
        return new Tweet
        {
            AuthorId = authorId,
            Text = text,
            PubDate = pubDate,
            Flagged = flagged
        };
    }

    public static Tweet Create(string authorId, string text)
    {
        return Create(authorId, text, DateTime.UtcNow, false);
    }
}