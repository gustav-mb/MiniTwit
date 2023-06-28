import { FormEvent, useEffect, useState } from "react";
import { getUserId, getUsername } from "../../authentication/JwtToken";
import TweetService from "../../services/TweetService";
import { TweetDTO } from "../../models/TweetDTO";
import TweetCollection from "../tweet/TweetCollection";
import { useDocumentTitle } from "../../utilities/Utilities";

const tweetService = new TweetService()

function MyTimeline({ setFlash }: { setFlash: (message: string) => void }) {
    useDocumentTitle("My Timeline")

    const [tweets, setTweets] = useState<TweetDTO[]>([])
    const [text, setText] = useState('')

    useEffect(() => {
        const fetchTweets = async () => {
            await tweetService.getTimeline(getUserId())
                .then(tweets => setTweets(tweets))
                .catch(_ => true)
        }
        
        fetchTweets()
    }, [])

    async function submit(e: FormEvent) {
        e.preventDefault() // Prevent page from reloading on submit
        
        const userId = getUserId()

        await tweetService.createTweet(userId, text)
            .then(() => setFlash("Your message was recorded"))
            .catch(() => true)
    }

    return (
        <>
            <h2>My Timeline</h2>
            <div className="twitbox">
                <h3>What's on your mind {getUsername()}?</h3>
                <form onSubmit={submit}>
                    <input type="text" name="text" size={60} onChange={e => setText(e.target.value)}></input>
                    <input type="submit" value="Share"></input>
                </form>
            </div>
            <TweetCollection tweets={tweets}></TweetCollection>
        </>
    );
}

export default MyTimeline