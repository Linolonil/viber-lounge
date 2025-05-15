import axios from 'axios';

export const API_URL = import.meta.env.VITE_API_URL ;

const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});



export default apiClient;