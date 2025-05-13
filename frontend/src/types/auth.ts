export interface User {
  id: string;
  nome: string;
  email: string;
  role: 'ADMIN' | 'USER';
}

export interface LoginCredentials {
  email: string;
  senha: string;
}

export interface AuthContextType {
  user: User | null;
  token: string | null;
  isInitializing: boolean;
  isAuthenticated: boolean;
  login: (email: string, senha: string) => Promise<void>;
  logout: () => void;
  register: (nome: string, email: string, senha: string) => Promise<void>;
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