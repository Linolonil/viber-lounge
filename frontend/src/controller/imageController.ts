import { imageService } from "@/services/ImageService"

export const imageController = {

  handleExcluir: async (filename: string) => {
    try {
     const pathname = new URL(filename).pathname;
      const filenameFormated = pathname.split('/').pop();
      const response = await imageService.deleteImage(filenameFormated);
      console.log("Imagem excluída com sucesso:", response);
      return response;
    } catch (error) {
      console.error("Erro ao excluir imagem:", error);
      throw error;
    }
  },

}