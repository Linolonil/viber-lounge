import api from './ApiUrl';
import { AuthResponse, LoginCredentials, RegisterData, User } from '../types/auth';

const authService = {
  async login(credentials: LoginCredentials): Promise<AuthResponse> {
    const response = await api.post(`/Auth/login`, credentials);
    const { token, user } = response.data;
    
    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(user));
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
    
    return { token, user };
  },

  async register(data: RegisterData): Promise<void> {
    await api.post(`/auth/register`, data);
  },

   logout(): void {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      delete api.defaults.headers.common['Authorization'];
  },

  async getCurrentUser(): Promise<User> {
    const response = await api.get(`/AuthProfile`);
    console.log('User data:', response.data);
    localStorage.setItem('user', JSON.stringify(response.data));
    return response.data;
  }
};

export default authService;