import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { toast } from "sonner";
import { Pencil, Trash2, Search, ImageIcon } from "lucide-react";
import { api } from "@/services/api";
import { ProductImage } from "@/components/ui/product-image";
import { produtoController } from "@/controller/produtoController";
import { Produto } from "@/types/Produtos";
import { imageController } from "@/controller/imageController";
import { apiClientImage } from "@/services/ApiUrl";

export default function ListaProdutos() {
  const [produtos, setProdutos] = useState<Produto[]>([]);
  const [query, setQuery] = useState('');
  const [produtoEditando, setProdutoEditando] = useState<Produto | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  // state para os campos do produto
  const [descricao, setDescricao] = useState("");
  const [descricaoLonga, setDescricaoLonga] = useState("");
  const [preco, setPreco] = useState("");
  const [quantidade, setQuantidade] = useState("");
  // img state
  const [imagem, setImagem] = useState<File | null>(null);
  const [previewImage, setPreviewImage] = useState<string | null>(null);
  const [imagemAntiga, setImagemAntiga] = useState<string | null>(null);
  
 useEffect(() => {
  if (!query) {
    carregarProdutos();
  } else {
    const buscar = async () => {
      const resultado = await produtosFiltrados(query);
      setProdutos(resultado);
    };
    buscar();
  }
}, [query]);

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setImagem(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        const base64String = reader.result as string;
        setPreviewImage(base64String);
      };
      reader.readAsDataURL(file);
    }
  };

  const carregarProdutos = async () => {
    try {
      const produtosCarregados = await produtoController.getProdutos();
      setProdutos(produtosCarregados);
    } catch (error) {
      console.log("Erro ao carregar produtos:", error);
    }
  };

  const handleSalvar = async () => {
  if (!produtoEditando) return;

  if (!descricao || !descricaoLonga || !preco || !quantidade) {
    toast.error("Nome, preço e quantidade são obrigatórios");
    return;
  }

  const precoNumerico = parseFloat(preco.replace(",", "."));
  const quantidadeNumerica = parseInt(quantidade);

  if (isNaN(precoNumerico) || precoNumerico <= 0) {
    toast.error("Preço inválido");
    return;
  }

  if (isNaN(quantidadeNumerica) || quantidadeNumerica < 0) {
    toast.error("Quantidade inválida");
    return;
  }

  try {
    let imagemFinal = produtoEditando.imagemUrl;

    // Se houver nova imagem diferente da anterior
    if (imagem && previewImage !== imagemAntiga) {
      // Supondo que você já fez o upload em outro momento
      const imageUploadForm = new FormData();
      imageUploadForm.append("image", imagem);

      const uploadResponse = await apiClientImage.post("/api/upload", imageUploadForm, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      const imageUrl = uploadResponse.data.imageUrl;
      imagemFinal = imageUrl;
    }

    const produtoAtualizado: Produto = {
      id: produtoEditando.id,
      descricao: descricao.trim(),
      descricaoLonga: descricaoLonga.trim(),
      preco: precoNumerico,
      quantidade: quantidadeNumerica,
      imagemUrl: imagemFinal,
    };

    await produtoController.updateProduto(produtoAtualizado);

    setDialogOpen(false);
    carregarProdutos();
  } catch (error) {
    console.error("Erro ao atualizar produto:", error);
    toast.error("Erro ao atualizar produto");
  }
};


  const handleExcluir = async (id: string, imagemUrl: string) => {
    if (confirm("Tem certeza que deseja excluir este produto?")) {
      try {
        await imageController.handleExcluir(imagemUrl);
        await produtoController.deleteProduto(id);

        carregarProdutos();
      } catch (error) {
        console.error("Erro ao excluir produto:", error);
      }
    }
  };

 const produtosFiltrados = async (query: string) => {
  if (!query) return [];
  try {
    const produtos = await produtoController.buscarProdutos(query);
    return produtos;  
  } catch (error) {
    console.error('Erro ao filtrar produtos:', error);
    return [];
  }
};

   const abrirDialogEdicao = (produto: Produto) => {
    setProdutoEditando(produto);
    setDescricao(produto.descricao);
    setDescricaoLonga(produto.descricaoLonga);
    setPreco(produto.preco.toString().replace(".", ","));
    setQuantidade(produto.quantidade.toString());
    setImagem(null);
    setPreviewImage(produto.imagemUrl);
    setImagemAntiga(produto.imagemUrl);
    setDialogOpen(true);
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h2 className="text-3xl font-semibold tracking-tight text-white font-['Poppins']">Produtos Cadastrados</h2>
          <p className="text-muted-foreground">Gerencie os produtos disponíveis no Viber Lounge.</p>
        </div>
        
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
          <Input
            placeholder="Buscar produtos..."
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            className="bg-zinc-700 border-zinc-600 text-white pl-9 w-full md:w-64"
          />
        </div>
      </div>
      
      {produtos.length === 0 ? (
        <div className="text-center p-8 bg-zinc-800 rounded-lg border border-zinc-700">
          <p className="text-gray-400">Nenhum produto encontrado. Cadastre um novo produto na seção de cadastro.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {produtos.map( produto => (
          <Card
            key={produto?.id}//ponto de melhoria é id
            className="bg-zinc-800 border-zinc-700 overflow-hidden flex flex-col"
          >
            <div className="relative aspect-video w-full overflow-hidden">
              <ProductImage
                src={produto?.imagemUrl ? `${produto?.imagemUrl}` : null}
                alt={produto?.descricao}
                className="w-full h-full object-contain object-top rounded-t-md"
              />
            </div>

            <CardHeader className="pb-2 space-y-1">
              <CardTitle className="text-white text-lg">{produto?.descricao}</CardTitle>
              {produto?.descricaoLonga && (
                <p className="text-sm text-gray-400">{produto.descricaoLonga}</p>
              )}
            </CardHeader>

            <CardContent className="pb-2">
              <div className="space-y-2">
                <div className="flex justify-between items-center">
                  <span className="text-gray-400 text-sm">Preço:</span>
                  <p className="text-viber-gold text-lg font-semibold">
                    R$ {produto?.preco.toFixed(2).replace(".", ",")}
                  </p>
                </div>
                <div className="flex justify-between items-center">
                  <span className="text-gray-400 text-sm">Quantidade:</span>
                  <p className="text-white text-lg font-semibold">
                    {produto?.quantidade} unidades
                  </p>
                </div>
              </div>
            </CardContent>

            <CardFooter className="flex justify-end gap-2 pt-2">
              <Button
                variant="outline"
                size="sm"
                className="border-zinc-600 text-gray-200 hover:bg-zinc-700 hover:text-white text-black"
                onClick={() => abrirDialogEdicao(produto)}
              >
                <Pencil className="h-4 w-4 mr-2" />
                Editar
              </Button>
              <Button
                variant="destructive"
                size="sm"
                onClick={() => handleExcluir(produto?.id, produto?.imagemUrl)}
              >
                <Trash2 className="h-4 w-4 mr-2" />
                Excluir
              </Button>
            </CardFooter>
          </Card>
        ))}

        </div>
      )}
    
      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent className="bg-zinc-800 text-white border-zinc-700 max-w-md">
          <DialogHeader>
            <DialogTitle>Editar Produto</DialogTitle>
          </DialogHeader>
          
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <Label htmlFor="edit-descricao" className="text-gray-200">Descrição do Produto</Label>
              <Input
                id="edit-descricao"
                value={descricao}
                onChange={(e) => setDescricao(e.target.value)}
                className="bg-zinc-700 border-zinc-600 text-white"
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="edit-descricao" className="text-gray-200">Descrição do Produto</Label>
              <Input
                id="edit-descricao-longa"
                value={descricaoLonga}
                onChange={(e) => setDescricaoLonga(e.target.value)}
                className="bg-zinc-700 border-zinc-600 text-white"
              />
            </div>
            
            <div className="space-y-2">
              <Label htmlFor="edit-preco" className="text-gray-200">Preço (R$)</Label>
              <Input
                id="edit-preco"
                value={preco}
                onChange={(e) => setPreco(e.target.value)}
                className="bg-zinc-700 border-zinc-600 text-white"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="edit-quantidade" className="text-gray-200">Quantidade</Label>
              <Input
                id="edit-quantidade"
                type="number"
                min="0"
                value={quantidade}
                onChange={(e) => setQuantidade(e.target.value)}
                className="bg-zinc-700 border-zinc-600 text-white"
              />
            </div>
            
            <div className="space-y-2">
              <Label htmlFor="edit-imagem" className="text-gray-200">Imagem do Produto</Label>
              <Input
                id="edit-imagem"
                type="file"
                accept="image/*"
                onChange={handleImageChange}
                className="bg-zinc-700 border-zinc-600 text-white"
              />
              
              <div className="mt-2 flex justify-center">
                {previewImage ? (
                  <img
                    src={previewImage.startsWith('data:') ? previewImage : `${previewImage}`}
                    alt="Preview"
                    className="max-h-32 max-w-full object-contain rounded-md"
                  />
                ) : (
                  <div className="flex flex-col items-center justify-center bg-zinc-900 rounded-lg p-4 text-gray-400 h-32 w-full">
                    <ImageIcon className="h-8 w-8 mb-2" />
                    <p className="text-sm">Sem imagem</p>
                  </div>
                )}
              </div>
            </div>
          </div>
          
          <DialogFooter>
            <Button 
              variant="outline" 
              onClick={() => setDialogOpen(false)}
              className="border-zinc-600 text-gray-200 hover:bg-zinc-700 text-black"
            >
              Cancelar
            </Button>
            <Button 
              onClick={handleSalvar}
              className="bg-viber-gold hover:bg-viber-gold/80 text-black"
            >
              Salvar Alterações
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
