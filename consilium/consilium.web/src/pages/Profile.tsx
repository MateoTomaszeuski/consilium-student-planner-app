import { useState, useEffect, useRef } from 'react';
import { useAuth } from '../hooks/useAuth';

export const Profile = () => {
  const { user, isAuthenticated, renderGoogleButton, signOut, isLoading } = useAuth();
  const [message, setMessage] = useState('');
  const buttonRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    // Clear button when authenticated or loading
    if ((isAuthenticated || isLoading) && buttonRef.current) {
      buttonRef.current.innerHTML = '';
      return;
    }
    
    // Render Google button when not authenticated and button div is available
    if (!isAuthenticated && !isLoading && buttonRef.current) {
      // Small delay to ensure DOM is ready
      const timer = setTimeout(() => {
        if (buttonRef.current) {
          renderGoogleButton(buttonRef.current);
        }
      }, 100);
      
      return () => clearTimeout(timer);
    }
  }, [isAuthenticated, isLoading, renderGoogleButton]);

  const handleLogout = () => {
    signOut();
    setMessage('Logged out successfully');
    // Clear message after 3 seconds
    setTimeout(() => setMessage(''), 3000);
  };

  if (isLoading) {
    return (
      <div className="max-w-4xl mx-auto">
        <h1 className="text-3xl font-bold mb-8 text-dark-dark text-center">Profile</h1>
        <div className="flex justify-center items-center py-12">
          <div className="text-xl text-dark-med">Loading...</div>
        </div>
      </div>
    );
  }

  if (isAuthenticated && user) {
    return (
      <div className="max-w-4xl mx-auto">
        <h1 className="text-3xl font-bold mb-8 text-dark-dark text-center">Profile</h1>
        <div className="bg-light-light border border-dark-med/20 rounded-xl p-8 shadow-sm max-w-md mx-auto">
          <h2 className="text-2xl font-bold mb-6 text-dark-dark text-center">Welcome, {user.displayName}!</h2>
          
          <div className="flex flex-col items-center space-y-4">
            {user.profilePicture && (
              <img 
                src={user.profilePicture} 
                alt="Profile" 
                className="w-24 h-24 rounded-full border-4 border-mid-green shadow-md"
              />
            )}
            
            <div className="w-full space-y-3 bg-white rounded-lg p-4 border border-dark-med/20">
              <div className="flex justify-between items-center pb-3 border-b border-dark-med/10">
                <span className="text-dark-med font-semibold">Email:</span>
                <span className="text-dark-dark">{user.email}</span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-dark-med font-semibold">Role:</span>
                <span className="text-dark-dark">{user.role === 0 ? 'User' : 'Admin'}</span>
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
        
        {message && (
          <div className="mt-6 p-4 rounded-lg text-center font-semibold bg-green-100 border border-green-300 text-green-800">
            {message}
          </div>
        )}
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto">
      <h1 className="text-3xl font-bold mb-8 text-dark-dark text-center">Profile</h1>
      <div className="bg-light-light border border-dark-med/20 rounded-xl p-8 shadow-sm max-w-md mx-auto">
        <h2 className="text-2xl font-bold mb-6 text-dark-dark text-center">Sign in with Google</h2>
        <div ref={buttonRef} className="flex justify-center"></div>
      </div>
      
      {message && (
        <div className="mt-6 p-4 rounded-lg text-center font-semibold bg-red-100 border border-red-300 text-red-800">
          {message}
        </div>
      )}
    </div>
  );
};
