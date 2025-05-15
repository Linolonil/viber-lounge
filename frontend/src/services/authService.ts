import axios from 'axios';
import { AuthResponse, LoginCredentials, RegisterData, User } from '../types/auth';
import apiClient from './ApiUrl';

const authService =
{
  async login(credentials: LoginCredentials): Promise<AuthResponse> {
    try {
      const response = await apiClient.post(`/Auth/login`, credentials);
      const { token, user } = response.data;

      return { token, user };
    } catch (error) {
      if (axios.isAxiosError(error)) {
        console.log(error);
        throw new Error(error.response?.data?.message || 'Servidor Indisponível no momento');
      }
      throw error;
    }
  },

  async register(data: RegisterData): Promise<User> {
    try {
      const response = await apiClient.post(`/Auth/register`, data);
      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error)) {
        console.log(error);
        throw new Error(error.response?.data?.message || 'Servidor Indisponível no momento');
      }
      throw error;
    }
  },

  async logout(): Promise<void> {
    delete apiClient.defaults.headers.common['Authorization'];
  },

  async getCurrentUser(token: string): Promise<User | null> {
    try {
      const token = localStorage.getItem('token');
      if (!token) return null;

      const response = await apiClient.get(`/AuthProfile`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      
      return response.data;
    } catch (error) {
      
      localStorage.removeItem('token');
      delete apiClient.defaults.headers.common['Authorization'];
      return null;
    }
  },
};

export default authService; 