import { useState, useEffect, useCallback } from 'react';
import { authService } from '../services/authService';

export const Profile = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(authService.isLoggedIn());
  const [message, setMessage] = useState('');
  const [user, setUser] = useState(authService.getStoredUser());

  const handleGoogleSignIn = useCallback(async (response: { credential: string }) => {
    try {
      const newUser = await authService.googleSignIn(response.credential);
      setUser(newUser);
      setIsLoggedIn(true);
      setMessage(`Welcome, ${newUser.displayName}!`);
      
      // Clean up Google Sign-In button after successful login
      const buttonDiv = document.getElementById('google-signin-button');
      if (buttonDiv) {
        buttonDiv.innerHTML = '';
      }
    } catch (error) {
      console.error('Google sign-in failed:', error);
      const errorMessage = error && typeof error === 'object' && 'response' in error 
        ? (error.response as { data?: string })?.data || 'Sign-in failed. Please try again.'
        : 'Sign-in failed. Please try again.';
      setMessage(errorMessage);
    }
  }, []);

  useEffect(() => {
    // Only initialize if not logged in
    if (isLoggedIn) return;

    // Initialize Google Sign-In
    const initializeGoogleSignIn = () => {
      const buttonDiv = document.getElementById('google-signin-button');
      
      if (window.google?.accounts?.id && buttonDiv) {
        const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;
        
        if (!clientId || clientId === 'YOUR_ACTUAL_CLIENT_ID.apps.googleusercontent.com') {
          console.error('VITE_GOOGLE_CLIENT_ID is not configured properly');
          setMessage('Google Sign-In is not configured. Please contact support.');
          return false;
        }
        
        window.google.accounts.id.initialize({
          client_id: clientId,
          callback: handleGoogleSignIn,
          auto_select: false,
        });

        // Render the button
        window.google.accounts.id.renderButton(
          buttonDiv,
          { 
            theme: 'outline', 
            size: 'large',
            text: 'signin_with',
            shape: 'rectangular',
          }
        );
        
        return true;
      }
      
      return false;
    };

    // Wait for both Google script and DOM element to be ready
    const checkReady = setInterval(() => {
      // Double-check login state before initializing
      if (authService.isLoggedIn()) {
        clearInterval(checkReady);
        setIsLoggedIn(true);
        setUser(authService.getStoredUser());
        return;
      }
      
      if (window.google?.accounts?.id && document.getElementById('google-signin-button')) {
        clearInterval(checkReady);
        initializeGoogleSignIn();
      }
    }, 100);

    // Cleanup
    return () => clearInterval(checkReady);
  }, [isLoggedIn, handleGoogleSignIn]);

  // Make the callback available globally for Google
  useEffect(() => {
    window.handleGoogleSignIn = handleGoogleSignIn;
    return () => {
      delete window.handleGoogleSignIn;
    };
  }, [handleGoogleSignIn]);

  const handleLogout = () => {
    authService.logOut();
    setUser(null);
    setIsLoggedIn(false);
    setMessage('Logged out successfully');
  };

  return (
    <div className="max-w-4xl mx-auto">
      <h1 className="text-3xl font-bold mb-8 text-dark-dark text-center">Profile</h1>

      {!isLoggedIn ? (
        <div className="bg-light-light border border-dark-med/20 rounded-xl p-8 shadow-sm max-w-md mx-auto">
          <h2 className="text-2xl font-bold mb-6 text-dark-dark text-center">Sign in with Google</h2>
          <div id="google-signin-button" className="flex justify-center"></div>
        </div>
      ) : (
        <div className="bg-light-light border border-dark-med/20 rounded-xl p-8 shadow-sm max-w-md mx-auto">
          <h2 className="text-2xl font-bold mb-6 text-dark-dark text-center">Welcome, {user?.displayName}!</h2>
          
          <div className="flex flex-col items-center space-y-4">
            {user?.profilePicture && (
              <img 
                src={user.profilePicture} 
                alt="Profile" 
                className="w-24 h-24 rounded-full border-4 border-mid-green shadow-md"
              />
            )}
            
            <div className="w-full space-y-3 bg-white rounded-lg p-4 border border-dark-med/20">
              <div className="flex justify-between items-center pb-3 border-b border-dark-med/10">
                <span className="text-dark-med font-semibold">Email:</span>
                <span className="text-dark-dark">{user?.email}</span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-dark-med font-semibold">Role:</span>
                <span className="text-dark-dark">{user?.role === 0 ? 'User' : 'Admin'}</span>
              </div>
            </div>

            <button 
              onClick={handleLogout} 
              className="w-full px-6 py-3 bg-red-600 hover:bg-red-700 text-white font-semibold rounded-lg transition-colors shadow-sm mt-4"
            >
              Logout
            </button>
          </div>
        </div>
      )}

      {message && (
        <div className={`mt-6 p-4 rounded-lg text-center font-semibold ${
          message.includes('failed') || message.includes('error')
            ? 'bg-red-100 border border-red-300 text-red-800' 
            : 'bg-green-100 border border-green-300 text-green-800'
        }`}>
          {message}
        </div>
      )}
    </div>
  );
};
