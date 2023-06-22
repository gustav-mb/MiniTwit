import { Md5 } from "ts-md5";
import { TweetDTO } from "../models/TweetDTO";

function Tweet(tweet: TweetDTO) {
    return (
        <li>
            <img src={ getGravatarURL(tweet.email, 48)}></img>
            <p>
                <strong><a href={tweet.username}>{ tweet.username }</a></strong>
            </p>
            { tweet.text }
            <small>&mdash; { formatDateTime(tweet.pubDate) }</small>
        </li>
    );
}

function formatDateTime(dateTime: string): string {
    return new Date(dateTime).toLocaleString()
}

function getGravatarURL(email: string, size: number = 80) {
    const hash = Md5.hashStr(email.trim().toLowerCase())
    return `http://www.gravatar.com/avatar/${hash}?d=identicon&s=${size}`
}

export default Tweet