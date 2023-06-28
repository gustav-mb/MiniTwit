import { useEffect, useState } from "react";
import { TweetDTO } from "../../models/TweetDTO";
import { useDocumentTitle } from "../../utilities/Utilities";
import TweetCollection from "../tweet/TweetCollection";
import TweetService from "../../services/TweetService";

const tweetService = new TweetService()

function PublicTimeline() {
    useDocumentTitle("Public Timeline")

    const [tweets, setTweets] = useState<TweetDTO[]>([])

    useEffect(() => {
        const fetchTweets = async () => {
            await tweetService.getPublicTimeline()
                .then(tweets => setTweets(tweets))
        }
        
        fetchTweets()
    }, [])

    return (
        <>
            <h2>Public Timeline</h2>
            <TweetCollection tweets={tweets}></TweetCollection>
        </>
    );
}

export default PublicTimeline