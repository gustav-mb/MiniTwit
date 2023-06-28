export interface TweetDTO
{
    id: string
    authorId: string
    authorName: string
    email: string
    text: string
    pubDate: string
}

export interface TweetCreateDTO
{
    authorId: string
    text: string
}