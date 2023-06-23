import { useLocation, useParams } from "react-router-dom";
import { getUsername } from "../../authentication/JwtToken";
import { useDocumentTitle } from "../../utilities/Utilities";

// TODO Implement follow logic
const followed = false

function UserTimeline() {
    const { username } = useParams()
    useDocumentTitle(`${username}'s Timeline`)
    
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
        </>
    );
}

function unfollowUser() {
    
}

function followUser() {

}

export default UserTimeline