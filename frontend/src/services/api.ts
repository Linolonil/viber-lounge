import { DadosGrafico, Produto, Venda, ItemVenda, FormaPagamento, PeriodoTrabalho } from "@/lib/types";
import axios from "axios";
import apiClient from "./ApiUrl";

export const API_URL = import.meta.env.VITE_API_URL ;
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

  createVenda: async (venda: Omit<Venda, 'id' | 'createdAt'>): Promise<Venda> => {
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

  cancelSale: async (id: string, motivo: string): Promise<Venda> => {
    const response = await fetch(`${API_URL}/vendas/${id}`, {
      method: 'DELETE',
      headers: getAuthHeaders(),
      body: JSON.stringify({ motivo })
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
  deleteImagemProduto: api.deleteImagemProduto,
};

export const VendaService = {
  getVendas: api.getVendas,
  getVenda: api.getVenda,
  createVenda: api.createVenda,
  getVendasByDate: api.getVendasByDate,
  cancelSale: api.cancelSale,
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
