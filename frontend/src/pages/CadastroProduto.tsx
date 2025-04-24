
import { useState } from "react";
import { ProdutoService, generateId } from "@/services/storage";
import { Produto } from "@/lib/types";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { toast } from "sonner";
import { ImageIcon, Save } from "lucide-react";
import { useQueryClient, useMutation } from "@tanstack/react-query";

export default function CadastroProduto() {
  const [nome, setNome] = useState("");
  const [preco, setPreco] = useState("");
  const [imagem, setImagem] = useState("");
  const [previewImage, setPreviewImage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const queryClient = useQueryClient();

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        const base64String = reader.result as string;
        setImagem(base64String);
        setPreviewImage(base64String);
      };
      reader.readAsDataURL(file);
    }
  };

  const cadastrarProdutoMutation = useMutation({
<<<<<<< HEAD
    mutationFn: (produto: Produto) => ProdutoService.createProduto(produto),
=======
    mutationFn: async (produto: Produto) => ProdutoService.save(produto),
>>>>>>> edf8ca1ad6b83234df4c3630d22b1a34d3a2dc71
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['produtos'] });
      toast.success("Produto cadastrado com sucesso!");
      
      // Limpar o formulário
      setNome("");
      setPreco("");
      setImagem("");
      setPreviewImage(null);
      
      // Resetar o input de arquivo
      const fileInput = document.getElementById("imagem") as HTMLInputElement;
      if (fileInput) fileInput.value = "";
      
      setIsSubmitting(false);
    },
    onError: (error) => {
      console.error("Erro ao cadastrar produto:", error);
      toast.error("Erro ao cadastrar produto");
      setIsSubmitting(false);
    }
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!nome || !preco || !imagem) {
      toast.error("Preencha todos os campos obrigatórios");
      return;
    }
    
    const precoNumerico = parseFloat(preco.replace(",", "."));
    
    if (isNaN(precoNumerico) || precoNumerico <= 0) {
      toast.error("Preço inválido");
      return;
    }
    
    setIsSubmitting(true);
    
    const novoProduto: Produto = {
      id: generateId(),
      nome,
      preco: precoNumerico,
      imagem,
      quantidade: 0,
    
    };
    
    cadastrarProdutoMutation.mutate(novoProduto);
  };

  return (
    <div className="space-y-6">
      <h2 className="text-3xl font-semibold tracking-tight text-white font-['Poppins']">Cadastro de Produto</h2>
      <p className="text-muted-foreground">Adicione novos produtos ao cardápio do Viber Lounge.</p>
      
      <Card className="bg-zinc-800 border-zinc-700">
        <CardHeader>
          <CardTitle className="text-white">Novo Produto</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <div className="space-y-2">
                  <Label htmlFor="nome" className="text-gray-200">Nome do Produto</Label>
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
                  <Label htmlFor="preco" className="text-gray-200">Preço (R$)</Label>
                  <Input
                    id="preco"
                    placeholder="Ex: 15,00"
                    value={preco}
                    onChange={(e) => setPreco(e.target.value)}
                    className="bg-zinc-700 border-zinc-600 text-white"
                    disabled={isSubmitting}
                  />
                </div>
                
                <div className="space-y-2">
                  <Label htmlFor="imagem" className="text-gray-200">Imagem do Produto</Label>
                  <div className="flex items-center gap-4">
                    <Input
                      id="imagem"
                      type="file"
                      accept="image/*"
                      onChange={handleImageChange}
                      className="bg-zinc-700 border-zinc-600 text-white"
                      disabled={isSubmitting}
                    />
                  </div>
                </div>
              </div>
              
              <div className="flex flex-col items-center justify-center bg-zinc-900 rounded-lg p-4 border border-dashed border-zinc-600">
                {previewImage ? (
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
                disabled={isSubmitting}
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
