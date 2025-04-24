import { DadosGrafico, Produto, Venda } from "@/lib/types";

const API_URL = 'http://localhost:3000/api';

interface ApiError {
  error: string;
}

const handleError = async (response: Response) => {
  if (!response.ok) {
    const errorData = await response.json() as ApiError;
    throw new Error(errorData.error || 'Erro na requisição');
  }
  return response;
};

export const api = {
  // Produtos
  getProdutos: async (): Promise<Produto[]> => {
    const response = await fetch(`${API_URL}/produtos`);
    await handleError(response);
    return response.json();
  },

  getProduto: async (id: string): Promise<Produto> => {
    const response = await fetch(`${API_URL}/produtos/${id}`);
    await handleError(response);
    return response.json();
  },

  createProduto: async (produto: Omit<Produto, 'id'>): Promise<Produto> => {
    const response = await fetch(`${API_URL}/produtos`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(produto),
    });
    await handleError(response);
    return response.json();
  },

  updateProduto: async (id: string, produto: Omit<Produto, 'id'>): Promise<Produto> => {
    const response = await fetch(`${API_URL}/produtos/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(produto),
    });
    await handleError(response);
    return response.json();
  },

  deleteProduto: async (id: string): Promise<void> => {
    const response = await fetch(`${API_URL}/produtos/${id}`, {
      method: 'DELETE',
    });
    await handleError(response);
  },

  // Vendas
  getVendas: async (): Promise<Venda[]> => {
    const response = await fetch(`${API_URL}/vendas`);
    await handleError(response);
    return response.json();
  },

  getVenda: async (id: string): Promise<Venda> => {
    const response = await fetch(`${API_URL}/vendas/${id}`);
    await handleError(response);
    return response.json();
  },

  createVenda: async (venda: Omit<Venda, 'id'>): Promise<Venda> => {
    const response = await fetch(`${API_URL}/vendas`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(venda),
    });
    await handleError(response);
    return response.json();
  },

  getVendasByDate: async (date: string): Promise<Venda[]> => {
    const response = await fetch(`${API_URL}/vendas/data/${date}`);
    await handleError(response);
    return response.json();
  },

  // Novos métodos de cancelamento
  cancelSale: async (id: string): Promise<Venda> => {
    const response = await fetch(`${API_URL}/vendas/${id}`, {
      method: 'DELETE',
    });
    await handleError(response);
    return response.json();
  },

  cancelItem: async (id: string, itemId: string, quantidade: number): Promise<Venda> => {
    const response = await fetch(`${API_URL}/vendas/${id}/itens/${itemId}`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ quantidade }),
    });
    await handleError(response);
    return response.json();
  },

  getDadosByDate: async (date: string): Promise<DadosGrafico> => {

    const response = await fetch(`${API_URL}/dashboard/${date}`);

    const responseJson = await response.json();
    await handleError(response);
    return responseJson;
  },

  getTraceByDate: async (date: Date): Promise<string> => {
    const response = await fetch(`${API_URL}/trace/${date.toISOString().split('T')[0]}`);
    await handleError(response);
    return response.text();
  },
};

export const ProdutoService = {
  getProdutos: api.getProdutos,
  getProduto: api.getProduto,
  createProduto: api.createProduto,
  updateProduto: api.updateProduto,
  deleteProduto: api.deleteProduto,
};

export const generateId = (): string => {
  return Math.random().toString(36).substr(2, 9);
};

export const DashboardService = {
  getVendas: api.getVendas,
  getVenda: api.getVenda,
  createVenda: api.createVenda,
  getVendasByDate: api.getVendasByDate,
  getDadosByDate: api.getDadosByDate,
};

export const TraceService = {
  cancelSale: api.cancelSale,
  cancelItem: api.cancelItem,
  getTraceByDate: api.getTraceByDate,
};
