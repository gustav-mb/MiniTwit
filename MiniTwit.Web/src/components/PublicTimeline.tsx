import { TweetDTO } from "../models/TweetDTO";
import { useDocumentTitle } from "../utilities/Utilities";
import TweetCollection from "./TweetCollection";

function PublicTimeline() {
    useDocumentTitle("Public Timeline")
    const tweets: TweetDTO[] = []

    return (
        <>
            <h2>Public Timeline</h2>
            <TweetCollection tweets={tweets}></TweetCollection>
        </>
    );
}

export default PublicTimeline