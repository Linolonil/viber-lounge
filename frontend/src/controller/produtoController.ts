import { useAuth } from "@/contexts/AuthContext";
import apiClient from "@/services/ApiUrl";
import produtoService from "@/services/ProdutoService";
import { Produto } from "@/types/Produtos";
import { toast } from "sonner";

export const produtoController = {
    
    getProdutos: async (): Promise<Produto[]> => {
    try {
      const produtos = await produtoService.getAll();

      return produtos;
    } catch (error) {
      toast.error(error?.response?.data.message || "Erro ao carregar produtos");
      throw error;  
    }
  },

//   getProduto: async (id: string): Promise<Produto> => {
//     const response = await fetch(`${API_URL}/produtos/${id}`, {
//       headers: `Bearer ${token}`
//     });
//     await handleError(response);
//     return response.json();
//   },

 buscarProdutos: async (query: string) => {
    try {
      const produtos = await produtoService.searchProdutos(query);
      return produtos;
    } catch (error) {
      console.error('Erro no controller ao buscar produtos:', error);
      throw error;
    }
  },

  updateProduto: async ( produto: Produto): Promise<Produto | null> => {
  try {
    const response = await produtoService.update(produto);
    toast.success("Produto atualizado com sucesso");
    return response;
  } catch (error: any) {
    console.error("Erro no updateProduto:", error);

    // Tenta extrair a mensagem do erro vindo do backend
    const mensagemErro =
      error?.response?.data?.message ||
      error?.message ||
      "Erro ao atualizar produto";

    toast.error(mensagemErro);
    return null;
  }
},

  createProduto: async (produto: Omit<Produto, 'id'>): Promise<Produto | null> => {
  try {
    const response = await produtoService.create(produto);
    return response;
  } catch (error) {
    console.error('Erro ao criar produto:', error);
    return null;
  }
},

  deleteProduto: async (id: string): Promise<void> => {
    try {
      const produtos = await produtoService.delete(id);
      toast.success(produtos);
    } catch (error) {
      toast.error(error?.response?.data.message || "Erro ao deletar produto");
      throw error;  
    }
  },


}