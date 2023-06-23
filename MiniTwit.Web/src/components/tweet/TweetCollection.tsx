import { TweetDTO } from "../../models/TweetDTO";
import Tweet from "./Tweet";

function TweetCollection({tweets}: {tweets: TweetDTO[]}) {
    return (
        <>
            <ul className="messages">
                {tweets.length !== 0
                    ?
                    <>
                        {tweets.map(tweet => (
                            <Tweet authorId={tweet.authorId} username={tweet.username} email={tweet.email} text={tweet.text} pubDate={tweet.pubDate}></Tweet>
                        ))
                        }
                    </>
                    :
                    <li>
                        <em>There's no message so far.</em>
                    </li>
                }
            </ul>
        </>
    );
}

export default TweetCollection