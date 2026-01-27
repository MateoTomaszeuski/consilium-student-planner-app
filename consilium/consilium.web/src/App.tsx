import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { useEffect } from 'react';
import { Layout } from './components/Layout';
import { Dashboard } from './pages/Dashboard';
import { TodoList } from './pages/TodoList';
import { Assignments } from './pages/Assignments';
import { Tools } from './pages/Tools';
import { Profile } from './pages/Profile';
import { Settings } from './pages/Settings';
import { useAppStore } from './store/appStore';
import { authService } from './services/authService';
import type { Theme } from './types';
import './App.css';

function App() {
  const { theme, setTheme } = useAppStore();

  useEffect(() => {
    // Load user's theme preference from stored user data
    const user = authService.getStoredUser();
    if (user?.themePreference) {
      setTheme(user.themePreference as Theme);
    }
  }, [setTheme]);

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme);
  }, [theme]);

  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/todos" element={<TodoList />} />
          <Route path="/assignments" element={<Assignments />} />
          <Route path="/tools" element={<Tools />} />
          <Route path="/profile" element={<Profile />} />
          <Route path="/settings" element={<Settings />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  );
}

export default App;
