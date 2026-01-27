import api from './api';

declare global {
  interface Window {
    gapi: any;
    google: any;
  }
}

export interface GoogleUser {
  email: string;
  displayName: string;
  profilePicture?: string;
  role: number;
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
};
