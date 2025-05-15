export interface Produto {
  id: string;
  descricao: string;
  descricaoLonga: string;
  preco: number;
  imagemUrl: string;
  quantidade: number; 
}

export interface ItemVenda {
  produtoId: string;
  quantidade: number;
  precoUnitario: number;
  produtoNome: string;
  subtotal: number;
  estoqueAntes: number;
  estoqueDepois: number;
  produto?: Produto; // Opcional para compatibilidade com o frontend
}

export type FormaPagamento = "pix" | "credito" | "debito" | "dinheiro";
export type StatusVenda = "ativa" | "cancelada";

export interface Venda {
  id: string;
  createdAt: string; // ISO string
  usuarioId: string;
  usuarioNome: string;
  terminalId?: string;
  periodoId: string;
  status: StatusVenda;
  canceladaPorId?: string;
  canceladaPorNome?: string;
  canceladaEm?: string;
  motivoCancelamento?: string;
  itens: ItemVenda[];
  total: number;
  formaPagamento: FormaPagamento;
  cliente?: string;
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
