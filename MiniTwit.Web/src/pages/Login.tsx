import { FormEvent, useState } from "react";
import { useDocumentTitle } from "../utilities/Utilities";
import AuthenticationService from "../services/AuthenticationService";
import { parseToken } from "../authentication/JwtToken";
import { useNavigate } from "react-router-dom";
import { APIError } from "../models/APIError";

const authService = new AuthenticationService();

function Login({ setFlash }: {setFlash: (message: string) => void}) {
    useDocumentTitle("Sign In")
    const navigate = useNavigate()

    const [username, setUsername] = useState<string>('')
    const [password, setPassword] = useState<string>('')
    const [error, setError] = useState<string | null>(null)

    async function submit(e: FormEvent) {
        e.preventDefault() // Prevent page from reloading on submit

        if (username === '') {
            setError('You have to enter a username')
            return
        }

        if (password === '') {
            setError('You have to enter a password')
            return
        }

        await authService.login(username, password)
            .then(token => {
                console.log(token)
                parseToken(token.accessToken)
                setFlash("You were logged in")
                navigate('/')
            })
            .catch((error: APIError) => setError(error.errorMsg))
    }

    return (
        <>
            <h2>Sign In</h2>
            {error !== null &&
                <div className="error">
                    <strong>Error:</strong> {error}
                </div>
            }
            <form onSubmit={submit}>
                <dl>
                    <dt>Username:</dt>
                    <dd>
                        <input type="text" name="username" size={30} onChange={e => setUsername(e.target.value)}></input>
                    </dd>
                    <dt>Password:</dt>
                    <dd>
                        <input type="password" name="password" size={30} onChange={e => setPassword(e.target.value)}></input>
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