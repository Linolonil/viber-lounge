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

export interface RegisterData {
  nome: string;
  email: string;
  senha: string;
  role?: 'ADMIN' | 'USER';
}

export interface AuthResponse {
  token: string;
  user: User;
} 


export interface AuthContextType {
  user: User | null;
  token: string | null;
  login: (email: string, senha: string) => Promise<void>;
  logout: () => void;
  isLoadingRegister: boolean;
  loadingUser: boolean;
  isLoadingLogin: boolean;
  register: (nome: string, email: string, senha: string) => Promise<void>;
}