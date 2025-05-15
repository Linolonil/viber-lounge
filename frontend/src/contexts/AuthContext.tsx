import React, { createContext, useContext, useState, useEffect } from 'react';
import { useQueryClient, useMutation, useQuery } from '@tanstack/react-query';
import { AuthContextType, User } from '../types/auth';
import authService from '../services/authService';
import { toast } from 'sonner';

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const queryClient = useQueryClient();

  // Gerenciar o token e o usuário no estado
  const [token, setToken] = useState<string | null>(() => localStorage.getItem('token'));
  const [isLoadingRegister, setLoadingRegister] = useState<boolean>(false);

  // 🔍 Buscar o usuário se o token existir
  const { data: user, isLoading: loadingUser,  } = useQuery<User | null>({
    queryKey: ['auth-user'],
    queryFn: async () => {
      if (!token) return null;
      return authService.getCurrentUser(token);
    },
    enabled: !!token, 
    staleTime: 1000 * 60 * 480, 
  });
  
  // Mutação de login
  const loginMutation = useMutation({
    mutationFn: ({ email, senha }: { email: string; senha: string }) =>
      authService.login({ email, senha }),
    
    onSuccess: ({ token }) => {
      localStorage.setItem('token', token);
      setToken(token);
      queryClient.invalidateQueries({ queryKey: ['auth-user'] }); 
      toast.success(`Login realizado com sucesso!`);
    },
    
    onError: (err) => {
      console.error('Erro no login', err);
      toast.error(err?.message || 'Erro desconhecido');
      throw err;
    },
  });
  
  const isLoadingLogin = loginMutation.status === 'pending';
  
  // Função de login
  const login = async (email: string, senha: string) => {
    await loginMutation.mutateAsync({ email, senha });
  };
  
  // Função de logout
  const logout = () => {
    authService.logout();
    setToken(null);
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    queryClient.setQueryData(['auth-user'], null);
    toast.success('Logout realizado com sucesso!');
    
  };
  
  // Função de registro
  const register = async (nome: string, email: string, senha: string) => {
    try {
      setLoadingRegister(true);
      await authService.register({ nome, email, senha });
      await login(email, senha); 
    } catch (error) {
      console.log(error);
      toast.error(error.message);
      throw error;
    } finally {
      setLoadingRegister(false);
    }
  };
  
  return (
    <AuthContext.Provider
    value={{
      user,
      token,
      isLoadingLogin,
      loadingUser,
      isLoadingRegister,
      login,
      logout,
      register,
    }}
    >
      {children}
    </AuthContext.Provider>
  );
};

// Hook customizado para acessar o contexto
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
