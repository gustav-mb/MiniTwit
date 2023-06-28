import { Md5 } from "ts-md5";
import { TweetDTO } from "../../models/TweetDTO";

function Tweet({tweet}: {tweet: TweetDTO}) {
    return (
        <>
        <li>
            <img src={ getGravatarURL(tweet.authorName, 48)} alt="avatar"></img>
            <p>
                <strong><a href={tweet.authorName}>{ tweet.authorName }</a></strong>
            </p>
            { tweet.text }
            <small>&mdash; { formatDateTime(tweet.pubDate) }</small>
        </li>
        </>
    );
}

function formatDateTime(dateTime: string): string {
    return new Date(dateTime).toLocaleString()
}

function getGravatarURL(authorName: string, size: number = 80) {
    const hash = Md5.hashStr(authorName.trim().toLowerCase())
    return `http://www.gravatar.com/avatar/${hash}?d=identicon&s=${size}`
}

export default Tweet