import api from './api';

export const authService = {
  async logIn(email: string): Promise<string> {
    const response = await api.get<string>(`/Account/login/${email}`);
    const token = response.data;
    
    if (token !== 'Too many unauthorized keys') {
      localStorage.setItem('consilium_email', email);
      localStorage.setItem('consilium_token', token);
    }
    
    return token;
  },

  async checkAuthStatus(): Promise<boolean> {
    try {
      const response = await api.get('/Account/validate');
      return response.status === 200;
    } catch {
      return false;
    }
  },

  async logOut(): Promise<void> {
    await api.get('/Account/logout');
    localStorage.removeItem('consilium_email');
    localStorage.removeItem('consilium_token');
  },

  async globalLogOut(): Promise<void> {
    await api.get('/Account/globalsignout');
    localStorage.removeItem('consilium_email');
    localStorage.removeItem('consilium_token');
  },

  getStoredEmail(): string {
    return localStorage.getItem('consilium_email') || '';
  },

  getStoredToken(): string {
    return localStorage.getItem('consilium_token') || '';
  },

  isLoggedIn(): boolean {
    return !!this.getStoredEmail() && !!this.getStoredToken();
  },
};
