import '../style.css';
import { Route, Routes } from "react-router-dom";
import { isLoggedIn } from "../authentication/JwtToken"
import Footer from "../components/Footer"
import Header from "../components/Header"
import Timeline from "./TimeLine";
import PublicTimeline from "../components/timelines/PublicTimeline";
import Login from "./Login";
import Register from "./Register";
import UserTimeline from "../components/timelines/UserTimeline";
import { useDocumentTitle } from "../utilities/Utilities";
import NotFound from './NotFound';
import { useState } from 'react';

function Layout() {
    useDocumentTitle("Welcome")

    const [flashMessage, setFlashMessage] = useState<string | null>(null)
    
    const setFlash = (message: string) => {
        setFlashMessage(message)
    }

    return (
        <div className="page">
            <Header isLoggedIn={isLoggedIn()}></Header>
            {flashMessage !== null &&
                <ul className="flashes">
                    <li>{ flashMessage }</li>
                </ul>
            }
            <div className="body">
                <Routes>
                    <Route path="/" element={<Timeline setFlash={setFlash}></Timeline>}></Route>
                    <Route path="/public" element={<PublicTimeline></PublicTimeline>}></Route>
                    <Route path="/login" element={<Login setFlash={setFlash}></Login>}></Route>
                    <Route path="/register" element={<Register setFlash={setFlash}></Register>}></Route>
                    <Route path="/:username" element={<UserTimeline setFlash={setFlash}></UserTimeline>}></Route>
                    <Route path="*" element={<NotFound></NotFound>}></Route>
                </Routes>
            </div>
            <Footer></Footer>
        </div>
    );
}

export default Layout