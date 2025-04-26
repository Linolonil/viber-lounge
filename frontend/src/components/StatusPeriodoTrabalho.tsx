import { usePeriodoTrabalho } from '../contexts/PeriodoTrabalhoContext';

export function StatusPeriodoTrabalho() {
  const { periodoAtual, loading } = usePeriodoTrabalho();

  if (loading) return null;

  const getStatusColor = () => {
    if (!periodoAtual) return 'bg-red-500';
    if (periodoAtual.status === 'encerrado') return 'bg-yellow-500';
    return 'bg-green-500';
  };

  const getStatusText = () => {
    if (!periodoAtual) return 'Período de Trabalho Inativo';
    if (periodoAtual.status === 'encerrado') return 'Período de Trabalho Encerrado';
    return 'Período de Trabalho Ativo';
  };

  return (
    <div className={`w-full h-2 ${getStatusColor()} transition-colors duration-300`}>
      <div className="container mx-auto px-4 h-full flex items-center justify-center">
        <span className="text-xs text-white font-medium">
          {getStatusText()}
        </span>
      </div>
    </div>
  );
} 