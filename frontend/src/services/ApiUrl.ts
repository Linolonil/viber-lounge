import axios from 'axios';

export const API_URL = import.meta.env.VITE_API_URL ;
export const VITE_API_IMG_URL = import.meta.env.VITE_API_IMG_URL ;

const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});
export const apiClientImage = axios.create({
  baseURL: VITE_API_IMG_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers['Authorization'] = `Bearer ${token}`;
  }
  return config;
}, (error) => {
  return Promise.reject(error);
});



export default apiClient;