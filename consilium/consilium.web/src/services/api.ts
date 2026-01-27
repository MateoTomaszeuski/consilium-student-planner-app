import axios from 'axios';

// Create axios instance with base URL
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'https://consilium-api-cpgdcqaxepbyc2gj.westus3-01.azurewebsites.net/',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add request interceptor to include user email
api.interceptors.request.use(
  (config) => {
    const userStr = localStorage.getItem('consilium_user');
    
    if (userStr) {
      try {
        const user = JSON.parse(userStr);
        if (user.email) {
          config.headers['Email-Auth_Email'] = user.email;
        }
      } catch (e) {
        console.error('Failed to parse user data', e);
      }
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default api;
