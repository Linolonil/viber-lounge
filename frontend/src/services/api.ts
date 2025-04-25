import { DadosGrafico, Produto, Venda, ItemVenda, FormaPagamento, PeriodoTrabalho } from "@/lib/types";

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:3001/api';
export const API_IMG = import.meta.env.VITE_API_IMG_URL;

interface ApiError {
  error: string;
}

const getAuthHeaders = () => {
  const token = localStorage.getItem('token');

  return {
    'Content-Type': 'application/json',
    'Authorization': token ? `Bearer ${token}` : ''
  };
};

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
    const response = await fetch(`${API_URL}/produtos`, {
      headers: getAuthHeaders()
    });
    await handleError(response);
    return response.json();
  },

  getProduto: async (id: string): Promise<Produto> => {
    const response = await fetch(`${API_URL}/produtos/${id}`, {
      headers: getAuthHeaders()
    });
    await handleError(response);
    return response.json();
  },

  createProduto: async (produto: Omit<Produto, 'id'>): Promise<Produto> => {
    const response = await fetch(`${API_URL}/produtos`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify(produto),
    });
    await handleError(response);
    return response.json();
  },

  createProdutoWithImage: async (formData: FormData): Promise<Produto> => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/produtos`, {
      method: 'POST',
      headers: {
        'Authorization': token ? `Bearer ${token}` : ''
      },
      body: formData
    });
    await handleError(response);
    return response.json();
  },

  updateProduto: async (id: string, produto: Omit<Produto, 'id'>): Promise<Produto> => {
    const response = await fetch(`${API_URL}/produtos/${id}`, {
      method: 'PUT',
      headers: getAuthHeaders(),
      body: JSON.stringify(produto),
    });
    await handleError(response);
    return response.json();
  },

  updateProdutoWithImage: async (id: string, formData: FormData): Promise<Produto> => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/produtos/${id}`, {
      method: 'PUT',
      headers: {
        'Authorization': token ? `Bearer ${token}` : ''
      },
      body: formData
    });
    await handleError(response);
    return response.json();
  },

  deleteProduto: async (id: string): Promise<void> => {
    const response = await fetch(`${API_URL}/produtos/${id}`, {
      method: 'DELETE',
      headers: getAuthHeaders()
    });
    await handleError(response);
  },

  // Vendas
  getVendas: async (): Promise<Venda[]> => {
    const response = await fetch(`${API_URL}/vendas`, {
      headers: getAuthHeaders()
    });
    
    if (response.status === 403) {
      throw new Error('Acesso negado. Apenas administradores podem acessar o histórico de vendas.');
    }
    
    await handleError(response);
    return response.json();
  },

  getVenda: async (id: string): Promise<Venda> => {
    const response = await fetch(`${API_URL}/vendas/${id}`, {
      headers: getAuthHeaders()
    });
    await handleError(response);
    return response.json();
  },

  createVenda: async (venda: Omit<Venda, 'id'>): Promise<Venda> => {
    const response = await fetch(`${API_URL}/vendas`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify(venda),
    });
    await handleError(response);
    return response.json();
  },

  getVendasByDate: async (date: string): Promise<Venda[]> => {
    const response = await fetch(`${API_URL}/vendas/data/${date}`, {
      headers: getAuthHeaders()
    });
    await handleError(response);
    return response.json();
  },

  // Novos métodos de cancelamento
  cancelSale: async (id: string): Promise<Venda> => {
    const response = await fetch(`${API_URL}/vendas/${id}`, {
      method: 'DELETE',
      headers: getAuthHeaders()
    });
    await handleError(response);
    return response.json();
  },

  cancelItem: async (id: string, itemId: string, quantidade: number): Promise<Venda> => {
    const response = await fetch(`${API_URL}/vendas/${id}/itens/${itemId}`, {
      method: 'DELETE',
      headers: getAuthHeaders(),
      body: JSON.stringify({ quantidade }),
    });
    await handleError(response);
    return response.json();
  },

  getDadosByDate: async (date: string): Promise<DadosGrafico> => {
    const response = await fetch(`${API_URL}/dashboard/${date}`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao buscar dados do dashboard');
    }
    return response.json();
  },

  getTraceByDate: async (date: Date): Promise<string> => {
    const response = await fetch(`${API_URL}/trace/${date.toISOString().split('T')[0]}`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao buscar trace de vendas');
    }
    return response.text();
  },

  getVendasPorPeriodo: async (periodoId: string): Promise<Venda[]> => {
    const response = await fetch(`${API_URL}/vendas/periodo/${periodoId}`, {
      headers: getAuthHeaders()
    });
    await handleError(response);
    return response.json();
  },
};

export const ProdutoService = {
  getProdutos: api.getProdutos,
  getProduto: api.getProduto,
  createProduto: api.createProduto,
  createProdutoWithImage: api.createProdutoWithImage,
  updateProduto: api.updateProduto,
  updateProdutoWithImage: api.updateProdutoWithImage,
  deleteProduto: api.deleteProduto,
};

export const VendaService = {
  getVendas: api.getVendas,
  getVenda: api.getVenda,
  createVenda: api.createVenda,
  getVendasByDate: api.getVendasByDate,
  cancelSale: api.cancelSale,
  cancelItem: api.cancelItem,
  getVendasPorPeriodo: api.getVendasPorPeriodo,
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

export const PeriodoTrabalhoService = {
  getPeriodoAtual: async (usuarioId: string): Promise<PeriodoTrabalho | null> => {
    const response = await fetch(`${API_URL}/periodo-trabalho/atual/${usuarioId}`, {
      headers: getAuthHeaders()
    });
    if (response.status === 404) return null;
    if (!response.ok) throw new Error('Erro ao buscar período de trabalho');
    return response.json();
  },

  iniciarPeriodo: async (usuarioId: string, usuarioNome: string): Promise<PeriodoTrabalho> => {
    const response = await fetch(`${API_URL}/periodo-trabalho/iniciar`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify({ usuarioId, usuarioNome }),
    });
    if (!response.ok) throw new Error('Erro ao iniciar período de trabalho');
    return response.json();
  },

  encerrarPeriodo: async (periodoId: string): Promise<PeriodoTrabalho> => {
    const response = await fetch(`${API_URL}/periodo-trabalho/encerrar/${periodoId}`, {
      method: 'POST',
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Erro ao encerrar período de trabalho');
    return response.json();
  },
};

export const generateId = (): string => {
  return Math.random().toString(36).substr(2, 9);
};
