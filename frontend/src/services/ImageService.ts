// services/ImageService.ts
import apiClient, { apiClientImage } from "./ApiUrl";

export const imageService = {
  deleteImage: async (filename: string) => {
    try {
      const response = await apiClientImage.delete(`/api/upload/${filename}`);
      return response.data;
    } catch (error) {
      console.error("Erro ao excluir imagem:", error);
      throw error;
    }
  }
};
