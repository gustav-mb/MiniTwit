using MiniTwit.Core.Entities;

namespace MiniTwit.Service.Data.DataCreators;

public static class TweetCreator
{
    public static Tweet Create(string authorId, string authorName, string text, DateTime pubDate, bool flagged = false)
    {
        return new Tweet
        {
            AuthorId = authorId,
            AuthorName = authorName,
            Text = text,
            PubDate = pubDate,
            Flagged = flagged
        };
    }

    public static Tweet Create(string authorId, string authorName, string text)
    {
        return Create(authorId, authorName, text, DateTime.UtcNow, false);
    }
}