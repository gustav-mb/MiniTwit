import { useDocumentTitle } from "../utilities/Utilities";

function Login() {
    useDocumentTitle("Sign In")

    return (
        <>
            <h2>Sign In</h2>
            <div className="error">
                <strong>Error:</strong> ERROR
            </div>
            <form>
                <dl>
                    <dt>Username:</dt>
                    <dd>
                        <input type="text" name="username" size={30}></input>
                    </dd>
                    <dt>Password:</dt>
                    <dd>
                        <input type="password" name="password" size={30}></input>
                    </dd>
                </dl>
                <div className="actions">
                    <input type="submit" value="Sign in"></input>
                </div>
            </form>
        </>
    );
}

export default Login