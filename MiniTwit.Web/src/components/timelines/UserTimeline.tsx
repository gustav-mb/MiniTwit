import { useParams } from "react-router-dom";
import { getUserId, getUsername } from "../../authentication/JwtToken";
import { useDocumentTitle } from "../../utilities/Utilities";
import FollowerService from "../../services/FollowerService";
import TweetService from "../../services/TweetService";
import { useEffect, useState } from "react";
import { TweetDTO } from "../../models/TweetDTO";
import TweetCollection from "../tweet/TweetCollection";

// TODO Implement follow logic
const followerService = new FollowerService()
const tweetService = new TweetService()

const followed = false

function UserTimeline({ setFlash }: { setFlash: (message: string) => void }) {
    const { username } = useParams()
    useDocumentTitle(`${username}'s Timeline`)

    const [tweets, setTweets] = useState<TweetDTO[]>([])

    useEffect(() => {
        const fetchTweets = async () => {
            await tweetService.getUserTimeline(username)
                .then(tweets => setTweets(tweets))
                .catch(_ => true)
        }
        
        fetchTweets()
    }, [username])

    async function followUser() {
        await followerService.followUser(username, getUserId())
            .then(() => setFlash(`You are now following "${username}"`))
            .catch(() => true)
    }

    async function unfollowUser() {
        await followerService.unfollowUser(username, getUserId())
            .then(() => setFlash(`You are no longer following "${username}"`))
            .catch(() => true)
    }
    
    return (
        <>
            <h2>{username}'s Timeline</h2>
            <div className="followstatus">
                {username === getUsername()
                    ?
                    <>This is you!</>
                    : followed
                        ?
                        <>
                            You are currently following this user.&nbsp;
                            <span className="unfollow follow-link" onClick={unfollowUser}>Unfollow user</span>
                        </>
                        :
                        <>
                            You are not yet following this user.&nbsp;
                            <span className="follow follow-link" onClick={followUser}>Follow user</span>
                        </>
                }
            </div>
            <TweetCollection tweets={tweets}></TweetCollection>
        </>
    );
}

export default UserTimeline