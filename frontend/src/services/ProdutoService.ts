import { Produto } from "@/types/Produtos";
import apiClient from "./ApiUrl";

const produtoService = {
  async getAll(): Promise<Produto[]> {
    try {
      const response = await apiClient.get('/Product');
      return response.data;
    } catch (error) {
      console.error("Erro ao buscar produtos da api ", { error });
      throw error;

    }
  },

  async getById(id: string, token: string): Promise<Produto> {
    const response = await apiClient.get(`/produtos/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  },

  searchProdutos: async (query: string) => {
    // Verifica se é somente números (id) ou contém letras (descrição)
    const isOnlyNumbers = /^\d+$/.test(query);
    console.log('isOnlyNumbers:', isOnlyNumbers);

    let url = '';
    if (isOnlyNumbers) {
      // Pesquisa por id
      url = `/Product/search?Id=${query}`;  // exemplo: busca por id direta
    } else {
      // Pesquisa por descrição (ou categoria)
      url = `/Product/search?Descricao=${encodeURIComponent(query)}`;
    }

    try {
      const response = await apiClient.get(url);
      return response.data; 
    } catch (error) {
      console.error('Erro ao buscar produtos:', error);
      throw error;
    }
  },

  async create(produto: Omit<Produto, 'id'>): Promise<Produto> {
    try {
      const response = await apiClient.post('/Product/create', produto)
      return response.data;
    } catch (error) {
      console.error('Erro ao enviar produto para API:', error);
      throw new Error('Erro ao criar produto');
    }
  },

  async update(produto: Produto): Promise<Produto> {
  try {
    const response = await apiClient.put(`/Product/update`, produto);
    return response.data;
  } catch (error) {
    console.error("Erro no service update:", error);
    throw error; 
  }
},

  async delete(id: string): Promise<string> {
    try {
      await apiClient.delete(`/Product/delete?id=${id}`);
      return 'Produto deletado com sucesso';
    } catch (error) {
      console.error("Erro ao deletar produto da API", { error });
      throw error;
    }
  }

};

export default produtoService;
