export interface Produto {
  id: string;
  nome: string;
  preco: number;
  imagemUrl: string; // Base64 ou URL
  quantidade: number; // Quantidade em estoque
}

export interface ItemVenda {
  produto: Produto;
  quantidade: number;
}

export type FormaPagamento = "pix" | "credito" | "debito" | "dinheiro";
export type StatusVenda = "ativa" | "cancelada";

export interface Venda {
  id: string;
  data: string; // ISO string
  itens: ItemVenda[];
  cliente: string;
  formaPagamento: FormaPagamento;
  total: number;
  status: StatusVenda;
}

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