import { Navigate } from "react-router-dom";
import { isLoggedIn } from "../authentication/JwtToken";
import MyTimeline from "../components/timelines/MyTimeLine";

function Timeline({ setFlash }: { setFlash: (message: string) => void }) {
    const loggedIn = isLoggedIn()

    return (
        <>
            <div className="body">
                {loggedIn
                    ?
                    <MyTimeline setFlash={setFlash}></MyTimeline>
                    :
                    <Navigate replace to="/public"></Navigate>
                }
            </div>
        </>
    );
}

export default Timeline