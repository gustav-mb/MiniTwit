namespace MiniTwit.Core.Entities;

public class Tweet
{
    string Id { get; set; } = null!;
    string AuthorId { get; set; } = null!;
    string Text { get; set; } = null!;
    DateTime PubDate { get; set; }
    bool Flagged { get; set; }
}