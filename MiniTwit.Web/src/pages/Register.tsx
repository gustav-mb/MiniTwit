import { FormEvent, useState } from "react";
import { useDocumentTitle } from "../utilities/Utilities";
import UserService from "../services/UserService";
import { useNavigate } from "react-router-dom";
import { APIError } from "../models/APIError";

const userService = new UserService()

function Register({ setFlash }: { setFlash: (message: string) => void }) {
    useDocumentTitle("Sign Up")
    const navigate = useNavigate()

    const [username, setUsername] = useState('')
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [passwordRepeat, setPasswordRepeat] = useState('')
    const [error, setError] = useState<string | null>(null)

    async function submit(e: FormEvent) {
        e.preventDefault() // Prevent page from reloading on submit

        if (username === '') {
            setError('You have to enter a username')
            return
        }

        if (!email.includes('@')) {
            setError('You have to enter a valid email address')
            return
        }

        if (password === '') {
            setError('You have to enter a password')
            return
        }

        if (password !== passwordRepeat) {
            setError('The two passwords do not match')
            return
        }

        await userService.register(username, password, email)
            .then(() => {
                setFlash("You were successfully registered and can login now")
                navigate('/login')
            })
            .catch((error: APIError) => setError(error.errorMsg))
    }

    return (
        <>
            <h2>Sign Up</h2>
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
                    <dt>E-Mail:</dt>
                    <dd>
                        <input type="text" name="email" size={30} onChange={e => setEmail(e.target.value)}></input>
                    </dd>
                    <dt>Password:</dt>
                    <dd>
                        <input type="password" name="password" size={30} onChange={e => setPassword(e.target.value)}></input>
                    </dd>
                    <dt>Password <small>(repeat)</small></dt>
                    <dd>
                        <input type="password" name="password2" size={30} onChange={e => setPasswordRepeat(e.target.value)}></input>
                    </dd>
                </dl>
                <div className="actions">
                    <input type="submit" value="Sign Up"></input>
                </div>
            </form>
        </>
    );
}

export default Register