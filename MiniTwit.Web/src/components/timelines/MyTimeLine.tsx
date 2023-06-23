import { getUsername } from "../../authentication/JwtToken";
import { useDocumentTitle } from "../../utilities/Utilities";

function MyTimeline() {
    useDocumentTitle("My Timeline")

    return (
         <>
            <h2>My Timeline</h2>
            <div className="twitbox">
                <h3>What's on your mind { getUsername() }?</h3>
                <form>
                    <input type="text" name="text" size={60}></input>
                    <input type="submit" value="Share"></input>
                </form>
            </div>
        </>
    );
}

export default MyTimeline