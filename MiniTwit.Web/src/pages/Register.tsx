import { useDocumentTitle } from "../utilities/Utilities";

function Register() {
    useDocumentTitle("Sign Up")

    return (
        <>
            <h2>Sign Up</h2>
            <div className="error">
                <strong>Error:</strong> ERROR
            </div>
            <dl>
                <dt>Username:</dt>
                <dd>
                    <input type="text" name="username" size={30}></input>
                </dd>
                <dt>E-Mail:</dt>
                <dd>
                    <input type="text" name="email" size={30}></input>
                </dd>
                <dt>Password:</dt>
                <dd>
                    <input type="password" name="password" size={30}></input>
                </dd>
                <dt>Password <small>(repeat)</small></dt>
                <dd>
                    <input type="password" name="password2" size={30}></input>
                </dd>
            </dl>
            <div className="actions">
                <input type="submit" value="Sign Up"></input>
            </div>
        </>
    );
}

export default Register