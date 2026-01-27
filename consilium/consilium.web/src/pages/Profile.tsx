import { useState } from 'react';
import { authService } from '../services/authService';

export const Profile = () => {
  const [email, setEmail] = useState('');
  const [isLoggedIn, setIsLoggedIn] = useState(authService.isLoggedIn());
  const [showUnauthorized, setShowUnauthorized] = useState(false);
  const [message, setMessage] = useState('');

  const handleLogin = async () => {
    if (!email || !isValidEmail(email)) {
      setMessage('Please enter a valid email');
      return;
    }

    try {
      const token = await authService.logIn(email);
      
      if (token === 'Too many unauthorized keys') {
        setMessage('Too many unauthorized keys. Please try again later.');
        return;
      }

      setIsLoggedIn(true);
      setMessage('Login successful! Check your email for authorization.');
      
      // Check authorization status
      const isAuthorized = await authService.checkAuthStatus();
      setShowUnauthorized(!isAuthorized);
      
      if (!isAuthorized) {
        setMessage('Please authorize the login from your email.');
      }
    } catch (error) {
      console.error('Login failed:', error);
      setMessage('Login failed. Please try again.');
    }
  };

  const handleLogout = async () => {
    try {
      await authService.logOut();
      setIsLoggedIn(false);
      setEmail('');
      setShowUnauthorized(false);
      setMessage('Logged out successfully');
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };

  const isValidEmail = (email: string): boolean => {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  };

  const username = authService.getStoredEmail().split('@')[0] || '';

  return (
    <div className="profile">
      <h1>Profile</h1>

      {!isLoggedIn ? (
        <div className="login-form">
          <h2>Login</h2>
          <input
            type="email"
            placeholder="Enter your email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleLogin()}
          />
          <button onClick={handleLogin} className="btn-primary">
            Login
          </button>
        </div>
      ) : (
        <div className="profile-info">
          <h2>Welcome, {username}!</h2>
          <p>Email: {authService.getStoredEmail()}</p>
          
          {showUnauthorized && (
            <div className="warning">
              <p>⚠️ Your login needs to be authorized. Please check your email.</p>
            </div>
          )}

          <button onClick={handleLogout} className="btn-danger">
            Logout
          </button>
        </div>
      )}

      {message && (
        <div className={`message ${message.includes('failed') || message.includes('unauthorized') ? 'error' : 'success'}`}>
          {message}
        </div>
      )}
    </div>
  );
};
