import React from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { useAuth } from '../contexts/AuthContext';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { PeriodoTrabalho } from '@/lib/types';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { toast } from 'sonner';
import { Clock, DollarSign, ShoppingCart, AlertCircle, Loader2 } from 'lucide-react';

export function PeriodoTrabalhoCard() {
  const { user, token } = useAuth();
  const queryClient = useQueryClient();

  const { data: periodoAtual, isLoading } = useQuery({
    queryKey: ['periodo-trabalho', user?.id],
    queryFn: async () => {
      if (!user?.id) throw new Error('Usuário não autenticado');
      if (!token) throw new Error('Token não fornecido');

      const response = await fetch(`http://localhost:3001/api/periodo-trabalho/atual/${user.id}`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Erro ao carregar período de trabalho');
      }
      return response.json();
    },
    enabled: !!user?.id && !!token
  });

  const iniciarPeriodo = useMutation({
    mutationFn: async () => {
      if (!token) throw new Error('Token não fornecido');

      const response = await fetch('http://localhost:3001/api/periodo-trabalho/iniciar', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({
          usuarioId: user?.id,
          usuarioNome: user?.email,
        }),
      });
      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Erro ao iniciar período de trabalho');
      }
      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['periodo-trabalho'] });
      toast.success('Período de trabalho iniciado com sucesso!');
    },
    onError: (error: Error) => {
      toast.error(error.message || 'Erro ao iniciar período de trabalho');
    },
  });

  const encerrarPeriodo = useMutation({
    mutationFn: async () => {
      if (!periodoAtual?.id) throw new Error('ID do período não encontrado');
      if (!token) throw new Error('Token não fornecido');

      const response = await fetch(`http://localhost:3001/api/periodo-trabalho/encerrar/${periodoAtual.id}`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Erro ao encerrar período de trabalho');
      }
      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['periodo-trabalho'] });
      toast.success('Período de trabalho encerrado com sucesso!');
    },
    onError: (error: Error) => {
      toast.error(error.message || 'Erro ao encerrar período de trabalho');
    },
  });

  if (isLoading) {
    return (
      <Card className="bg-zinc-800 border-zinc-700">
        <CardHeader>
          <CardTitle className="text-white">Período de Trabalho</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex items-center justify-center py-4">
            <Loader2 className="h-6 w-6 text-viber-gold animate-spin" />
            <span className="ml-2 text-gray-300">Carregando...</span>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="bg-zinc-800 border-zinc-700">
      <CardHeader>
        <CardTitle className="text-white">Período de Trabalho</CardTitle>
      </CardHeader>
      <CardContent>
        {periodoAtual ? (
          <div className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div className="flex items-center gap-2 text-gray-300">
                <Clock className="h-5 w-5 text-viber-gold" />
                <div>
                  <p className="text-sm">Início</p>
                  <p className="font-medium">
                    {format(new Date(periodoAtual.dataInicio), "dd/MM/yyyy HH:mm", { locale: ptBR })}
                  </p>
                </div>
              </div>
              <div className="flex items-center gap-2 text-gray-300">
                <ShoppingCart className="h-5 w-5 text-viber-gold" />
                <div>
                  <p className="text-sm">Total de Vendas</p>
                  <p className="font-medium">{periodoAtual.totalVendas}</p>
                </div>
              </div>
              <div className="flex items-center gap-2 text-gray-300">
                <DollarSign className="h-5 w-5 text-viber-gold" />
                <div>
                  <p className="text-sm">Valor Total</p>
                  <p className="font-medium">R$ {periodoAtual.totalValor.toFixed(2)}</p>
                </div>
              </div>
            </div>

            {periodoAtual.status === 'aberto' && (
              <div className="flex items-center gap-2 text-yellow-400 bg-yellow-400/10 p-3 rounded-md">
                <AlertCircle className="h-5 w-5" />
                <p>Período de trabalho em andamento</p>
              </div>
            )}

            <div className="flex justify-end">
              {periodoAtual.status === 'aberto' ? (
                <Button
                  onClick={() => encerrarPeriodo.mutate()}
                  className="bg-red-500 hover:bg-red-600"
                  disabled={encerrarPeriodo.isPending}
                >
                  {encerrarPeriodo.isPending ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Encerrando...
                    </>
                  ) : (
                    'Encerrar Período'
                  )}
                </Button>
              ) : (
                <Button
                  onClick={() => iniciarPeriodo.mutate()}
                  className="bg-viber-gold hover:bg-viber-gold/90 text-black"
                  disabled={iniciarPeriodo.isPending}
                >
                  {iniciarPeriodo.isPending ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Iniciando...
                    </>
                  ) : (
                    'Iniciar Novo Período'
                  )}
                </Button>
              )}
            </div>
          </div>
        ) : (
          <div className="text-center py-4">
            <p className="text-gray-400 mb-4">Nenhum período de trabalho em andamento</p>
            <Button
              onClick={() => iniciarPeriodo.mutate()}
              className="bg-viber-gold hover:bg-viber-gold/90 text-black"
              disabled={iniciarPeriodo.isPending}
            >
              {iniciarPeriodo.isPending ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Iniciando...
                </>
              ) : (
                'Iniciar Período de Trabalho'
              )}
            </Button>
          </div>
        )}
      </CardContent>
    </Card>
  );
} 