import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { API_URL, VendaService } from '@/services/api';
import { useAuth } from '@/contexts/AuthContext';
import { toast } from 'sonner';
import { PeriodoTrabalhoCard } from './PeriodoTrabalho';

export function Venda() {
  const { user } = useAuth();
  const queryClient = useQueryClient();
  const [showPeriodoMessage, setShowPeriodoMessage] = useState(false);

  const { data: periodoAtual } = useQuery({
    queryKey: ['periodo-trabalho', user?.id],
    queryFn: async () => {
      if (!user?.id) throw new Error('Usuário não autenticado');
      const response = await fetch(`${API_URL}/periodo-trabalho/atual/${user.id}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });
      if (response.status === 404) return null;
      if (!response.ok) throw new Error('Erro ao buscar período de trabalho');
      return response.json();
    },
    enabled: !!user?.id
  });

  const createVenda = useMutation({
    mutationFn: async (vendaData: any) => {
      const response = await fetch('http://localhost:3001/api/vendas', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(vendaData),
      });

      if (!response.ok) {
        const error = await response.json();
        if (error.message.includes('período de trabalho')) {
          setShowPeriodoMessage(true);
          throw new Error('É necessário iniciar um período de trabalho antes de realizar vendas');
        }
        throw new Error(error.message || 'Erro ao criar venda');
      }

      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['vendas'] });
      queryClient.invalidateQueries({ queryKey: ['periodo-trabalho'] });
      toast.success('Venda realizada com sucesso!');
    },
    onError: (error: Error) => {
      toast.error(error.message);
    },
  });

  return (
    <div className="space-y-6">
      <PeriodoTrabalhoCard />

      {showPeriodoMessage && (
        <div className="bg-yellow-500/10 border border-yellow-500/20 rounded-lg p-4 text-yellow-500">
          <p className="font-medium">Período de Trabalho Necessário</p>
          <p className="text-sm mt-1">
            Para realizar vendas, você precisa iniciar um período de trabalho primeiro.
            Use o card acima para iniciar seu período de trabalho.
          </p>
        </div>
      )}

    </div>
  );
} 