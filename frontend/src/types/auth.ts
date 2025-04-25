export interface User {
  id: string;
  nome: string;
  email: string;
  role: 'admin' | 'user';
}

export interface LoginCredentials {
  email: string;
  senha: string;
}

export interface RegisterData {
  nome: string;
  email: string;
  senha: string;
  role?: 'admin' | 'user';
}

export interface AuthResponse {
  token: string;
  user: User;
} 