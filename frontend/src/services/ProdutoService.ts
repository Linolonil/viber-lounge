import { Produto } from "@/lib/types";

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:3001/api';

const getAuthHeaders = () => {
  const token = localStorage.getItem('token');
  return {
    'Authorization': token ? `Bearer ${token}` : ''
  };
};

export class ProdutoService {
  static async getAll(): Promise<Produto[]> {
    const response = await fetch(`${API_URL}/produtos`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao buscar produtos');
    }
    return response.json();
  }

  static async getById(id: string): Promise<Produto> {
    const response = await fetch(`${API_URL}/produtos/${id}`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao buscar produto');
    }
    return response.json();
  }

  static async create(formData: FormData): Promise<Produto> {
    const response = await fetch(`${API_URL}/produtos`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: formData
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao cadastrar produto');
    }

    return response.json();
  }

  static async update(id: string, formData: FormData): Promise<Produto> {
    const response = await fetch(`${API_URL}/produtos/${id}`, {
      method: 'PUT',
      headers: getAuthHeaders(),
      body: formData
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao atualizar produto');
    }

    return response.json();
  }

  static async delete(id: string): Promise<void> {
    const response = await fetch(`${API_URL}/produtos/${id}`, {
      method: 'DELETE',
      headers: getAuthHeaders()
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao deletar produto');
    }
  }
} 