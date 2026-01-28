import api from './api';

declare global {
  interface Window {
    gapi: unknown;
    google?: {
      accounts?: {
        id?: {
          initialize: (config: {
            client_id: string;
            callback: (response: { credential: string }) => void;
            auto_select?: boolean;
            use_fedcm_for_prompt?: boolean;
          }) => void;
          renderButton: (element: HTMLElement, options: Record<string, unknown>) => void;
          disableAutoSelect: () => void;
        };
      };
    };
    handleGoogleSignIn?: (response: { credential: string }) => void;
  }
}

export interface GoogleUser {
  email: string;
  displayName: string;
  profilePicture?: string;
  role: number;
  themePreference?: string;
  notes?: string;
}

export const authService = {
  async googleSignIn(idToken: string): Promise<GoogleUser> {
    const response = await api.post<GoogleUser>('/Account/google-signin', {
      idToken
    });
    
    const user = response.data;
    localStorage.setItem('consilium_user', JSON.stringify(user));
    
    return user;
  },

  async getCurrentUser(email: string): Promise<GoogleUser | null> {
    try {
      const response = await api.get<GoogleUser>(`/Account/user?email=${email}`);
      return response.data;
    } catch {
      return null;
    }
  },

  logOut(): void {
    localStorage.removeItem('consilium_user');
    
    // Sign out from Google
    if (window.google?.accounts?.id) {
      window.google.accounts.id.disableAutoSelect();
    }
  },

  getStoredUser(): GoogleUser | null {
    const userStr = localStorage.getItem('consilium_user');
    if (!userStr) return null;
    
    try {
      return JSON.parse(userStr);
    } catch {
      return null;
    }
  },

  getStoredEmail(): string {
    const user = this.getStoredUser();
    return user?.email || '';
  },

  isLoggedIn(): boolean {
    return !!this.getStoredUser();
  },

  async updateTheme(theme: string): Promise<boolean> {
    try {
      await api.post('/Account/theme', { theme });
      
      // Update stored user with new theme
      const user = this.getStoredUser();
      if (user) {
        user.themePreference = theme;
        localStorage.setItem('consilium_user', JSON.stringify(user));
      }
      
      return true;
    } catch {
      return false;
    }
  },

  async updateNotes(notes: string): Promise<boolean> {
    try {
      await api.post('/Account/notes', { notes });
      
      // Update stored user with new notes
      const user = this.getStoredUser();
      if (user) {
        user.notes = notes;
        localStorage.setItem('consilium_user', JSON.stringify(user));
      }
      
      return true;
    } catch {
      return false;
    }
  },
};
