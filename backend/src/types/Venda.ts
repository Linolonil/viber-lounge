import { Produto } from '../types';

export interface ItemVenda {
  produto: Produto;
  quantidade: number;
  produtoId?: string;
  precoUnitario?: number;
}

export interface Venda {
  id: string;
  data: string;
  itens: ItemVenda[];
  total: number;
  usuarioId: string;
  usuarioNome: string;
  periodoId: string;
  status: 'ativa' | 'cancelada';
  formaPagamento: 'pix' | 'credito' | 'debito' | 'dinheiro';
  cliente?: string;
} 