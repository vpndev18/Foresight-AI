import { useState } from 'react';
import { isAuthenticated } from './services/api';
import LoginPage from './pages/LoginPage';
import Dashboard from './pages/Dashboard';

function App() {
  const [authed, setAuthed] = useState(isAuthenticated());

  if (!authed) {
    return <LoginPage onAuth={() => setAuthed(true)} />;
  }

  return <Dashboard onLogout={() => setAuthed(false)} />;
}

export default App;
