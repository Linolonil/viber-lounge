import { Venda, ItemVenda } from './Venda';

export interface Produto {
  id: string;
  nome: string;
  preco: number;
  imagemUrl: string; // Base64 ou URL
  quantidade: number; // Quantidade em estoque
  status: 'disponivel' | 'indisponivel'; // Status do produto
}

export type FormaPagamento = "pix" | "credito" | "debito" | "dinheiro";
export type StatusVenda = "ativa" | "cancelada";

export type { Venda, ItemVenda };

export interface DadosGrafico {
  pix: number;
  credito: number;
  debito: number;
  dinheiro: number;
  totalVendas: number;
  produtosMaisVendidos: {
    nome: string, 
    quantidade: number
  }[];
} 