import { useState, useEffect } from 'react';
import { authService } from '../services/authService';

declare global {
  interface Window {
    handleGoogleSignIn?: (response: any) => void;
  }
}

export const Profile = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(authService.isLoggedIn());
  const [message, setMessage] = useState('');
  const user = authService.getStoredUser();

  useEffect(() => {
    // Only initialize if not logged in
    if (isLoggedIn) return;

    // Initialize Google Sign-In
    const initializeGoogleSignIn = () => {
      if (window.google?.accounts?.id) {
        const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;
        
        window.google.accounts.id.initialize({
          client_id: clientId,
          callback: handleGoogleSignIn,
        });

        // Render the button
        const buttonDiv = document.getElementById('google-signin-button');
        if (buttonDiv) {
          window.google.accounts.id.renderButton(
            buttonDiv,
            { 
              theme: 'outline', 
              size: 'large',
              text: 'signin_with',
              shape: 'rectangular',
            }
          );
        }
      }
    };

    // Wait for Google script to load
    const checkGoogleLoaded = setInterval(() => {
      if (window.google?.accounts?.id) {
        clearInterval(checkGoogleLoaded);
        initializeGoogleSignIn();
      }
    }, 100);

    // Cleanup
    return () => clearInterval(checkGoogleLoaded);
  }, [isLoggedIn]);

  const handleGoogleSignIn = async (response: any) => {
    try {
      const user = await authService.googleSignIn(response.credential);
      setIsLoggedIn(true);
      setMessage(`Welcome, ${user.displayName}!`);
    } catch (error: any) {
      console.error('Google sign-in failed:', error);
      setMessage(error.response?.data || 'Sign-in failed. Please try again.');
    }
  };

  // Make the callback available globally for Google
  useEffect(() => {
    window.handleGoogleSignIn = handleGoogleSignIn;
    return () => {
      delete window.handleGoogleSignIn;
    };
  }, []);

  const handleLogout = () => {
    authService.logOut();
    setIsLoggedIn(false);
    setMessage('Logged out successfully');
  };

  return (
    <div className="profile">
      <h1>Profile</h1>

      {!isLoggedIn ? (
        <div className="login-form">
          <h2>Sign in with Google</h2>
          <div id="google-signin-button" style={{ marginTop: '20px' }}></div>
        </div>
      ) : (
        <div className="profile-info">
          <h2>Welcome, {user?.displayName}!</h2>
          {user?.profilePicture && (
            <img 
              src={user.profilePicture} 
              alt="Profile" 
              style={{ 
                width: '100px', 
                height: '100px', 
                borderRadius: '50%',
                marginBottom: '20px' 
              }} 
            />
          )}
          <p>Email: {user?.email}</p>
          <p>Role: {user?.role === 0 ? 'User' : 'Admin'}</p>

          <button onClick={handleLogout} className="btn-danger" style={{ marginTop: '20px' }}>
            Logout
          </button>
        </div>
      )}

      {message && (
        <div className={`message ${message.includes('failed') ? 'error' : 'success'}`}>
          {message}
        </div>
      )}
    </div>
  );
};
