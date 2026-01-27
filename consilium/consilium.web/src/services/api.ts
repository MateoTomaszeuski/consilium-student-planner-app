import axios from 'axios';

// Create axios instance with base URL
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'https://consilium-api-cpgdcqaxepbyc2gj.westus3-01.azurewebsites.net/',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add request interceptor to include auth token
api.interceptors.request.use(
  (config) => {
    const email = localStorage.getItem('consilium_email');
    const token = localStorage.getItem('consilium_token');
    
    if (email && token) {
      config.headers['Email'] = email;
      config.headers['Key'] = token;
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default api;
