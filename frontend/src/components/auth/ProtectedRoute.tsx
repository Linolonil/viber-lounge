import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';

interface ProtectedRouteProps {
  children: React.ReactNode;
  requireAdmin?: boolean;
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({
  children,
}) => {

  const { user, isLoadingLogin, loadingUser} = useAuth();

  // Mostra um loading enquanto verifica o auth (evita flash de conteúdo)
  if (isLoadingLogin || loadingUser ) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <span className="text-zinc-200 text-lg">Carregando...</span>
      </div>
    );
  }

  // Se não há usuário no contexto (não autenticado)
  if (!user) {
    return <Navigate to="/login" replace />;
  };

  // Acesso permitido
  return <>{children}</>;
};
