import { useState } from "react";
import { generateId, ProdutoService } from "@/services/api";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { toast } from "sonner";
import { ImageIcon, Save, Loader2 } from "lucide-react";
import { useQueryClient, useMutation } from "@tanstack/react-query";

export default function CadastroProduto() {
  const [nome, setNome] = useState("");
  const [preco, setPreco] = useState("");
  const [quantidade, setQuantidade] = useState("");
  const [imagemFile, setImagemFile] = useState<File | null>(null);
  const [previewImage, setPreviewImage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isLoadingImage, setIsLoadingImage] = useState(false);
  const queryClient = useQueryClient();

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      // Verifica se o arquivo é uma imagem
      if (!file.type.startsWith('image/')) {
        toast.error("Por favor, selecione um arquivo de imagem válido");
        return;
      }
      
      setIsLoadingImage(true);
      setImagemFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setPreviewImage(reader.result as string);
        setIsLoadingImage(false);
      };
      reader.onerror = () => {
        toast.error("Erro ao carregar a imagem");
        setIsLoadingImage(false);
      };
      reader.readAsDataURL(file);
    }
  };

  const cadastrarProdutoMutation = useMutation({
    mutationFn: async (formData: FormData) => {
      setIsSubmitting(true);
      try {
        return await ProdutoService.createProdutoWithImage(formData);
      } catch (error) {
        throw error;
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['produtos'] });
      toast.success("Produto cadastrado com sucesso!");
      resetForm();
    },
    onError: (error: Error) => {
      console.error("Erro ao cadastrar produto:", error);
      toast.error(error.message || "Erro ao cadastrar produto");
    },
    onSettled: () => {
      setIsSubmitting(false);
    }
  });

  const resetForm = () => {
    setNome("");
    setPreco("");
    setQuantidade("");
    setImagemFile(null);
    setPreviewImage(null);
    setIsLoadingImage(false);
    
    const fileInput = document.getElementById("imagem") as HTMLInputElement;
    if (fileInput) fileInput.value = "";
  };

  const validateFields = () => {
    if (!nome.trim()) {
      toast.error("Informe o nome do produto");
      return false;
    }

    const precoNumerico = parseFloat(preco.replace(",", "."));
    if (isNaN(precoNumerico) || precoNumerico <= 0) {
      toast.error("Informe um preço válido (maior que zero)");
      return false;
    }

    const quantidadeNumerica = parseInt(quantidade);
    if (isNaN(quantidadeNumerica) || quantidadeNumerica < 0) {
      toast.error("Informe uma quantidade válida (não negativa)");
      return false;
    }

    if (!imagemFile) {
      toast.error("Selecione uma imagem para o produto");
      return false;
    }

    return true;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateFields()) return;
    
    const precoNumerico = parseFloat(preco.replace(",", "."));
    const quantidadeNumerica = parseInt(quantidade);
    
    const formData = new FormData();
    formData.append('id', generateId());
    formData.append('nome', nome.trim());
    formData.append('preco', precoNumerico.toString());
    formData.append('quantidade', quantidadeNumerica.toString());
    formData.append('imagem', imagemFile as File);
    
    cadastrarProdutoMutation.mutate(formData);
  };

  return (
    <div className="space-y-6">
      <h2 className="text-3xl font-semibold tracking-tight text-white font-['Poppins']">
        Cadastro de Produto
      </h2>
      <p className="text-muted-foreground">
        Adicione novos produtos ao cardápio do Viber Lounge.
      </p>
      
      <Card className="bg-zinc-800 border-zinc-700">
        <CardHeader>
          <CardTitle className="text-white">Novo Produto</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="nome" className="text-gray-200">
                    Nome do Produto*
                  </Label>
                  <Input
                    id="nome"
                    placeholder="Ex: Gin Tônica"
                    value={nome}
                    onChange={(e) => setNome(e.target.value)}
                    className="bg-zinc-700 border-zinc-600 text-white"
                    disabled={isSubmitting}
                  />
                </div>
                
                <div className="space-y-2">
                  <Label htmlFor="preco" className="text-gray-200">
                    Preço (R$)*
                  </Label>
                  <Input
                    id="preco"
                    placeholder="Ex: 15,00"
                    value={preco}
                    onChange={(e) => setPreco(e.target.value.replace(/[^0-9,]/g, ''))}
                    className="bg-zinc-700 border-zinc-600 text-white"
                    disabled={isSubmitting}
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="quantidade" className="text-gray-200">
                    Quantidade em Estoque*
                  </Label>
                  <Input
                    id="quantidade"
                    type="number"
                    min="0"
                    placeholder="Ex: 10"
                    value={quantidade}
                    onChange={(e) => setQuantidade(e.target.value)}
                    className="bg-zinc-700 border-zinc-600 text-white"
                    disabled={isSubmitting}
                  />
                </div>
                
                <div className="space-y-2">
                  <Label htmlFor="imagem" className="text-gray-200">
                    Imagem do Produto*
                  </Label>
                  <Input
                    id="imagem"
                    type="file"
                    accept="image/*"
                    onChange={handleImageChange}
                    className="bg-zinc-700 border-zinc-600 text-white file:text-white"
                    disabled={isSubmitting || isLoadingImage}
                  />
                </div>
              </div>
              
              <div className="flex flex-col items-center justify-center bg-zinc-900 rounded-lg p-4 border border-dashed border-zinc-600">
                {isLoadingImage ? (
                  <div className="flex flex-col items-center justify-center text-gray-400 h-48">
                    <Loader2 className="w-12 h-12 mb-2 animate-spin" />
                    <p>Carregando imagem...</p>
                  </div>
                ) : previewImage ? (
                  <img
                    src={previewImage}
                    alt="Preview"
                    className="max-h-48 max-w-full object-contain rounded-md"
                  />
                ) : (
                  <div className="flex flex-col items-center justify-center text-gray-400 h-48">
                    <ImageIcon className="w-12 h-12 mb-2" />
                    <p>Preview da imagem</p>
                  </div>
                )}
              </div>
            </div>
            
            <div className="flex justify-end">
              <Button 
                type="submit" 
                className="bg-viber-gold hover:bg-viber-gold/80 text-black"
                disabled={isSubmitting || isLoadingImage}
              >
                <Save className="h-4 w-4 mr-2" />
                {isSubmitting ? "Salvando..." : "Salvar Produto"}
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}