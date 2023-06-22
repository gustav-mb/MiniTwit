export interface TweetDTO
{
    authorId: string
    username: string
    email: string
    text: string
    pubDate: string
}

export interface TweetCreateDTO
{
    authorId: string
    text: string
}