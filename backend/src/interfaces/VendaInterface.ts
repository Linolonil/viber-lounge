import { Venda } from "../types/Venda";

export interface DadosGrafico {
    pix: number;
    credito: number;
    debito: number;
    dinheiro: number;
    totalVendas: number;
    produtosMaisVendidos: Array<{
      id: string;
      nome: string;
      quantidade: number;
    }>;
  }
  
  export interface VendasPorUsuario {
    usuarioId: string;
    usuarioNome: string;
    totalVendas: number;
    totalValor: number;
    vendas: Venda[];
  }
  
  export interface PeriodoTrabalho {
    id: string;
    usuarioId: string;
    usuarioNome: string;
    dataInicio: string;
    dataFim?: string;
    status: 'aberto' | 'fechado';
    totalVendas: number;
    totalValor: number;
  }