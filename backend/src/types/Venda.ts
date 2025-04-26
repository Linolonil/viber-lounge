import { Produto } from '../types';

export interface ItemVenda {
  produtoId: string;
  produtoNome: string;
  precoUnitario: number;
  quantidade: number;
  subtotal: number;
  estoqueAntes: number;
  estoqueDepois: number;
}

export interface Venda {
  id: string;
  createdAt: string; // ISO completo (YYYY-MM-DDTHH:mm:ss.sssZ)
  usuarioId: string;
  usuarioNome: string;
  terminalId?: string;
  periodoId: string;
  status: 'ativa' | 'cancelada';
  canceladaPorId?: string;
  canceladaPorNome?: string;
  canceladaEm?: string;
  motivoCancelamento?: string;
  itens: ItemVenda[];
  total: number;
  formaPagamento: 'pix' | 'credito' | 'debito' | 'dinheiro';
  cliente?: string;
} 