import React, { createContext, useContext, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { usePeriodoTrabalho } from './PeriodoTrabalhoContext';
import { toast } from 'sonner';

interface VendaContextType {
  verificarAcesso: () => boolean;
}

const VendaContext = createContext<VendaContextType | undefined>(undefined);

export const VendaProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const navigate = useNavigate();
  const { periodoAtual } = usePeriodoTrabalho();

  const verificarAcesso = () => {
    if (!periodoAtual) {
      toast.error("É necessário iniciar um turno para realizar vendas");
      navigate("/dashboard");
      return false;
    }
    return true;
  };

  useEffect(() => {
    verificarAcesso();
  }, [periodoAtual]);

  return (
    <VendaContext.Provider value={{ verificarAcesso }}>
      {children}
    </VendaContext.Provider>
  );
};

export const useVenda = () => {
  const context = useContext(VendaContext);
  if (context === undefined) {
    throw new Error('useVenda must be used within a VendaProvider');
  }
  return context;
}; 