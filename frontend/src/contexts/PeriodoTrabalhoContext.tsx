import React, { createContext, useContext, useState, useEffect } from 'react';
import { PeriodoTrabalho } from '../lib/types';
import { PeriodoTrabalhoService } from '../services/api';
import { useAuth } from './AuthContext';
import { toast } from 'sonner';

interface PeriodoTrabalhoContextType {
  periodoAtual: PeriodoTrabalho | null;
  loading: boolean;
  iniciarJornada: () => Promise<void>;
  encerrarJornada: () => Promise<void>;
  verificarPeriodoAtivo: () => Promise<void>;
}

const PeriodoTrabalhoContext = createContext<PeriodoTrabalhoContextType | undefined>(undefined);

export const PeriodoTrabalhoProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { user } = useAuth();
  const [periodoAtual, setPeriodoAtual] = useState<PeriodoTrabalho | null>(null);
  const [loading, setLoading] = useState(true);

  const verificarPeriodoAtivo = async () => {
    if (!user?.id) return;
    
    try {
      const periodo = await PeriodoTrabalhoService.getPeriodoAtual(user.id);
      setPeriodoAtual(periodo);
    } catch (error) {
      console.error('Erro ao verificar período de trabalho:', error);
      toast.error('Erro ao verificar período de trabalho');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    verificarPeriodoAtivo();
  }, [user?.id]);

  const iniciarJornada = async () => {
    if (!user?.id || !user?.email) return;
    
    try {
      setLoading(true);
      const novoPeriodo = await PeriodoTrabalhoService.iniciarPeriodo(user.id, user.email);
      setPeriodoAtual(novoPeriodo);
      toast.success('Turno iniciado com sucesso!');
      
      // Força uma atualização imediata do estado
      await verificarPeriodoAtivo();
    } catch (error) {
      console.error('Erro ao iniciar turno:', error);
      toast.error('Erro ao iniciar turno');
    } finally {
      setLoading(false);
    }
  };

  const encerrarJornada = async () => {
    if (!periodoAtual?.id) return;
    
    try {
      setLoading(true);
      const periodoEncerrado = await PeriodoTrabalhoService.encerrarPeriodo(periodoAtual.id);
      setPeriodoAtual(periodoEncerrado);
      toast.success('Turno encerrado com sucesso!');
      
      // Força uma atualização imediata do estado
      await verificarPeriodoAtivo();
    } catch (error) {
      console.error('Erro ao encerrar turno:', error);
      toast.error('Erro ao encerrar turno');
    } finally {
      setLoading(false);
    }
  };

  return (
    <PeriodoTrabalhoContext.Provider 
      value={{ 
        periodoAtual, 
        loading, 
        iniciarJornada, 
        encerrarJornada,
        verificarPeriodoAtivo
      }}
    >
      {children}
    </PeriodoTrabalhoContext.Provider>
  );
};

export const usePeriodoTrabalho = () => {
  const context = useContext(PeriodoTrabalhoContext);
  if (context === undefined) {
    throw new Error('usePeriodoTrabalho must be used within a PeriodoTrabalhoProvider');
  }
  return context;
}; 