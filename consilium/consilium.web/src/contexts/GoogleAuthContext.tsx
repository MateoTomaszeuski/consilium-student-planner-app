import { createContext, useEffect, useState, useCallback } from 'react';
import type { ReactNode } from 'react';
import { authService, type GoogleUser } from '../services/authService';

interface GoogleButtonConfiguration {
  theme?: 'outline' | 'filled_blue' | 'filled_black';
  size?: 'large' | 'medium' | 'small';
  text?: 'signin_with' | 'signup_with' | 'continue_with' | 'signin';
  shape?: 'rectangular' | 'pill' | 'circle' | 'square';
  logo_alignment?: 'left' | 'center';
  width?: number;
}

interface CredentialResponse {
  credential: string;
  select_by?: string;
}

interface AuthContextType {
  user: GoogleUser | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  isSigningIn: boolean;
  signIn: () => void;
  signOut: () => void;
  renderGoogleButton: (element: HTMLElement, options?: GoogleButtonConfiguration) => void;
  updateTheme: (theme: string) => Promise<boolean>;
  updateNotes: (notes: string) => Promise<boolean>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export { AuthContext };

interface GoogleAuthProviderProps {
  children: ReactNode;
}

export function GoogleAuthProvider({ children }: GoogleAuthProviderProps) {
  const [user, setUser] = useState<GoogleUser | null>(authService.getStoredUser());
  const [isLoading, setIsLoading] = useState(true);
  const [isSigningIn, setIsSigningIn] = useState(false);
  const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;

  const handleCredentialResponse = useCallback(async (response: CredentialResponse) => {
    setIsSigningIn(true);
    try {
      const newUser = await authService.googleSignIn(response.credential);
      setUser(newUser);
      // Reload page to ensure clean state after sign-in
      window.location.reload();
    } catch (error) {
      console.error('[GoogleAuth] Error processing credential:', error);
      setIsSigningIn(false);
      // Don't update user state on error
    }
  }, []);

  useEffect(() => {
    // Check for existing user
    const storedUser = authService.getStoredUser();
    if (storedUser) {
      setUser(storedUser);
    }

    // Load Google Sign-In script
    const script = document.createElement('script');
    script.src = 'https://accounts.google.com/gsi/client';
    script.async = true;
    script.defer = true;
    script.onload = () => {
      if (window.google?.accounts?.id && clientId && clientId !== 'YOUR_ACTUAL_CLIENT_ID.apps.googleusercontent.com') {
        window.google.accounts.id.initialize({
          client_id: clientId,
          callback: handleCredentialResponse,
          auto_select: false,
          use_fedcm_for_prompt: true,
        });
        setIsLoading(false);
      } else {
        console.error('[GoogleAuth] Client ID not configured properly');
        setIsLoading(false);
      }
    };
    script.onerror = () => {
      console.error('[GoogleAuth] Failed to load Google Sign-In script');
      setIsLoading(false);
    };
    document.body.appendChild(script);

    return () => {
      if (document.body.contains(script)) {
        document.body.removeChild(script);
      }
    };
  }, [clientId, handleCredentialResponse]);

  const signIn = () => {
    if (window.google?.accounts?.id) {
      // @ts-expect-error - prompt method exists but may not be in type definitions
      window.google.accounts.id.prompt();
    }
  };

  const signOut = () => {
    authService.logOut();
    setUser(null);
  };

  const renderGoogleButton = (element: HTMLElement, options?: GoogleButtonConfiguration) => {
    if (window.google?.accounts?.id && !isLoading) {
      window.google.accounts.id.renderButton(element, {
        theme: 'outline',
        size: 'large',
        text: 'signin_with',
        shape: 'rectangular',
        ...options,
      });
    }
  };

  const updateTheme = async (theme: string): Promise<boolean> => {
    const result = await authService.updateTheme(theme);
    if (result) {
      // Update local user state
      const updatedUser = authService.getStoredUser();
      if (updatedUser) {
        setUser(updatedUser);
      }
    }
    return result;
  };

  const updateNotes = async (notes: string): Promise<boolean> => {
    const result = await authService.updateNotes(notes);
    if (result) {
      // Update local user state
      const updatedUser = authService.getStoredUser();
      if (updatedUser) {
        setUser(updatedUser);
      }
    }
    return result;
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: !!user,
        isLoading,
        isSigningIn,
        signIn,
        signOut,
        renderGoogleButton,
        updateTheme,
        updateNotes,
      }}
    >
      {isSigningIn && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-8 shadow-xl flex flex-col items-center space-y-4">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-mid-green"></div>
            <p className="text-lg font-semibold text-dark-dark">Signing you in...</p>
          </div>
        </div>
      )}
      {children}
    </AuthContext.Provider>
  );
}
