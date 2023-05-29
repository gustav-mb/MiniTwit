namespace MiniTwit.Core.DTOs;

public record TweetDTO
{
    public string AuthorName { get; init; } = null!;
    public string Text { get; init; } = null!;
    public DateTime PubDate { get; init; }
}

public record TweetCreateDTO
{
    public string AuthorId { get; init; } = null!;
    public string Text { get; init; } = null!;
}