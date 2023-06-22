import { getUsername } from "../authentication/JwtToken";

function Header({isLoggedIn}: {isLoggedIn: boolean}) {
    return (
        <>
            <h1>MiniTwit</h1>
            <div className="navigation">
                {isLoggedIn
                    ?
                    <>
                        <a href="/">my timeline</a> |
                        <a href="public">public timeline</a> |
                        <a href="/">sign out [{ getUsername() }]</a>
                    </>
                    :
                    <>
                        <a href="public">public timeline</a> |
                        <a href="register">sign up</a> |
                        <a href="login">sign in</a>
                    </>
                }
            </div>
        </>
    );
}

export default Header