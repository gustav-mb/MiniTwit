import axiosClient from "../utilities/Axios"

const ROUTE = 'Follower'

class FollowerService {
    public async followUser(username: string, userId: string): Promise<undefined> {
        return await axiosClient.post(`${ROUTE}/${username}/follow?userId=${userId}`)
    }

    public async unfollowUser(username: string, userId: string): Promise<undefined> {
        return await axiosClient.delete(`${ROUTE}/${username}/unfollow?userId=${userId}`)
    }
}

export default FollowerService