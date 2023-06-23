import './App.css';
import './style.css';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Timeline from './pages/TimeLine';
import PublicTimeline from './components/PublicTimeline';
import Login from './pages/Login';
import Register from './pages/Register';
import UserTimeline from './components/timelines/UserTimeline';
import Footer from './components/Footer';
import Header from './components/Header';
import { isLoggedIn } from './authentication/JwtToken';

function App() {
  // useDocumentTitle("Welcome | MiniTwit")

  return (
    <>
      <div className="page">
        <Header isLoggedIn={isLoggedIn()}></Header>
        <div className="body">
          <BrowserRouter>
            <Routes>
              <Route path="/" element={<Timeline></Timeline>}></Route>
              <Route path="/public" element={<PublicTimeline></PublicTimeline>}></Route>
              <Route path="/login" element={<Login></Login>}></Route>
              <Route path="/register" element={<Register></Register>}></Route>
              <Route path="/:username" element={<UserTimeline></UserTimeline>}></Route>
            </Routes>
          </BrowserRouter>
        </div>
        <Footer></Footer>
      </div>
    </>
  );
}

export default App;
