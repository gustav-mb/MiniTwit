import { Navigate } from "react-router-dom";
import { isLoggedIn } from "../authentication/JwtToken";
import MyTimeline from "../components/timelines/MyTimeLine";

function Timeline() {
    const loggedIn = isLoggedIn()

    return (
        <>
            {loggedIn
                ?
                    <MyTimeline></MyTimeline>
                :
                    <Navigate replace to="/public"></Navigate>
            }
        </>
    );
}

export default Timeline