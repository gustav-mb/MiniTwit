import './App.css';
import { BrowserRouter } from 'react-router-dom';
import Layout from './pages/Layout';

function App() {
  return (
    <>
        <BrowserRouter>
          <Layout></Layout>
        </BrowserRouter>
    </>
  );
}

export default App;
