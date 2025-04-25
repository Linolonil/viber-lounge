export interface Usuario {
  id: string;
  nome: string;
  email: string;
  senha: string;
  role: 'admin' | 'user';
}

export interface UsuarioLogin {
  email: string;
  senha: string;
}

export interface UsuarioResponse {
  id: string;
  nome: string;
  email: string;
  role: 'admin' | 'user';
} 