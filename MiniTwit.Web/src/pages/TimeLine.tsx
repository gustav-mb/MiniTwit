import { Navigate } from "react-router-dom";
import MyTimeline from "../components/MyTimeLine";
import { isLoggedIn } from "../authentication/JwtToken";

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