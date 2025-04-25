export interface Produto {
  id: string;
  nome: string;
  preco: number;
  quantidade: number;
  imagemUrl?: string;
  createdAt: Date;
  updatedAt: Date;
} 