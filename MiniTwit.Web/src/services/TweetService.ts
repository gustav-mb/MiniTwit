import { TweetCreateDTO, TweetDTO } from "../models/TweetDTO"
import axiosClient from "../utilities/Axios"

const ROUTE = 'Tweet'

class TweetService {
    public async getTimeline(userId: string, limit?: number): Promise<TweetDTO[]> {
        return await axiosClient.get(`${ROUTE}?userId=${userId}&limit=${limit}`)
    }

    public async getPublicTimeline(limit?: number): Promise<TweetDTO[]> {
        return await axiosClient.get(`${ROUTE}/public?limit=${limit}`)
    }

    public async getUserTimeline(username: string, limit?: number): Promise<TweetDTO[]> {
        return await axiosClient.get(`${ROUTE}/${username}?limit=${limit}`)
    }

    public async createTweet(authorId: string, text: string): Promise<undefined> {
        const tweetCreateDTO: TweetCreateDTO = {
            authorId: authorId,
            text: text
        }
        
        return await axiosClient.post(`${ROUTE}/add_message`, tweetCreateDTO)
    }
}

export default TweetService