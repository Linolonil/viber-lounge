import React, { createContext, useContext } from 'react';
import { AuthContextType, User } from '../types/auth';
import authService from '../services/authService';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';


const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const queryClient = useQueryClient();

  // Query para obter o usuário atual
  const { data: user, isLoading: isInitializing } = useQuery<User | null>({
    queryKey: ['currentUser'],
    queryFn: async () => {
      const token = localStorage.getItem('token');
      if (!token) return null;

      try {
        const user = await authService.getCurrentUser();
        return user;
      } catch (error) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        return null;
      }
    },
    staleTime: 1000 * 60 * 30, // 30 minutos
  });

  // Query para obter o token (armazenado no localStorage)
  const { data: token } = useQuery<string | null>({
    queryKey: ['authToken'],
    queryFn: () => localStorage.getItem('token'),
    initialData: localStorage.getItem('token') || null,
  });

  // Mutação para login
  const loginMutation = useMutation({
    mutationFn: (credentials: { email: string; senha: string }) => 
      authService.login(credentials),
    onSuccess: (data) => {
      queryClient.setQueryData(['currentUser'], data.user);
      queryClient.setQueryData(['authToken'], data.token);
    },
  });

  // Mutação para logout
  const logoutMutation = useMutation({
    mutationFn:  () => {
      authService.logout();
      return null;
    },
    onSuccess: () => {
      queryClient.setQueryData(['currentUser'], null);
      queryClient.setQueryData(['authToken'], null);
      queryClient.clear();
    },
  });

  // Mutação para registro
  const registerMutation = useMutation({
    mutationFn: (data: { nome: string; email: string; senha: string }) => 
      authService.register(data),
  });

  // Funções de conveniência para o contexto
  const login =  async (email: string, senha: string) => {
   await  loginMutation.mutateAsync({ email, senha });
  };

   const logout = (): void => {
     logoutMutation.mutateAsync();
  };

  const register = async (nome: string, email: string, senha: string) => {
    await registerMutation.mutateAsync({ nome, email, senha });
  };

  const isAuthenticated = !!user ;

  return (
    <AuthContext.Provider value={{ 
      user, 
      token, 
      isInitializing,
      isAuthenticated,
      login, 
      logout, 
      register 
    }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};