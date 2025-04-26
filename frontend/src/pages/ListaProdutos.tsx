import { useState, useEffect } from "react";
import { Produto } from "@/lib/types";
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
import { api, API_IMG } from "@/services/api";
import { ProductImage } from "@/components/ui/product-image";

export default function ListaProdutos() {
  const [produtos, setProdutos] = useState<Produto[]>([]);
  const [filtro, setFiltro] = useState("");
  const [produtoEditando, setProdutoEditando] = useState<Produto | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [nome, setNome] = useState("");
  const [preco, setPreco] = useState("");
  const [quantidade, setQuantidade] = useState("");
  const [imagem, setImagem] = useState<File | null>(null);
  const [previewImage, setPreviewImage] = useState<string | null>(null);
  const [imagemAntiga, setImagemAntiga] = useState<string | null>(null);
  
  useEffect(() => {
    carregarProdutos();
  }, []);

  const carregarProdutos = async () => {
    try {
      const produtosCarregados = await api.getProdutos();
      setProdutos(produtosCarregados);
    } catch (error) {
      console.error("Erro ao carregar produtos:", error);
      toast.error("Erro ao carregar produtos");
    }
  };


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

  const abrirDialogEdicao = (produto: Produto) => {
    setProdutoEditando(produto);
    setNome(produto.nome);
    setPreco(produto.preco.toString().replace(".", ","));
    setQuantidade(produto.quantidade.toString());
    setImagem(null);
    setPreviewImage(produto.imagemUrl);
    setImagemAntiga(produto.imagemUrl);
    setDialogOpen(true);
  };

  const handleSalvar = async () => {
    if (!produtoEditando) return;
    
    if (!nome || !preco || !quantidade) {
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
      // Verifica se há uma nova imagem e se é diferente da atual
      if (imagem && previewImage !== imagemAntiga) {
        // Se houver nova imagem, usa FormData
        const formData = new FormData();
        formData.append('nome', nome);
        formData.append('preco', precoNumerico.toString());
        formData.append('quantidade', quantidadeNumerica.toString());
        formData.append('imagem', imagem);
        
        await api.updateProdutoWithImage(produtoEditando.id, formData);
      } else {
        // Se não houver nova imagem ou for a mesma, usa JSON
        const produtoAtualizado: Produto = {
          id: produtoEditando.id,
          nome,
          preco: precoNumerico,
          imagemUrl: produtoEditando.imagemUrl,
          quantidade: quantidadeNumerica
        };
        
        await api.updateProduto(produtoAtualizado.id, produtoAtualizado);
      }
      
      toast.success("Produto atualizado com sucesso!");
      setDialogOpen(false);
      carregarProdutos();
    } catch (error) {
      console.error("Erro ao atualizar produto:", error);
      toast.error("Erro ao atualizar produto");
    }
  };

  const handleExcluir = async (id: string) => {
    if (confirm("Tem certeza que deseja excluir este produto?")) {
      try {
        await api.deleteProduto(id);
        toast.success("Produto excluído com sucesso!");
        carregarProdutos();
      } catch (error) {
        console.error("Erro ao excluir produto:", error);
        toast.error("Erro ao excluir produto");
      }
    }
  };

  const produtosFiltrados = produtos.filter(p => 
    p.nome.toLowerCase().includes(filtro.toLowerCase())
  );

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
            value={filtro}
            onChange={(e) => setFiltro(e.target.value)}
            className="bg-zinc-700 border-zinc-600 text-white pl-9 w-full md:w-64"
          />
        </div>
      </div>
      
      {produtosFiltrados.length === 0 ? (
        <div className="text-center p-8 bg-zinc-800 rounded-lg border border-zinc-700">
          <p className="text-gray-400">Nenhum produto encontrado. Cadastre um novo produto na seção de cadastro.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {produtosFiltrados.map(produto => (
            
            <Card key={produto.id} className="bg-zinc-800 border-zinc-700 overflow-hidden flex flex-col">
              <div className="relative aspect-video w-full overflow-hidden">
                <ProductImage
                  src={produto.imagemUrl ? `${API_IMG}${produto.imagemUrl}` : null}
                  alt={produto.nome}
                  className="w-full h-full"
                />
              </div>
              <CardHeader className="pb-2">
                <CardTitle className="text-white text-lg">{produto.nome}</CardTitle>
              </CardHeader>
              <CardContent className="pb-2">
                <div className="space-y-2">
                  <div className="flex justify-between items-center">
                    <span className="text-gray-400 text-sm">Preço:</span>
                    <p className="text-viber-gold text-lg font-semibold">
                      R$ {produto.preco.toFixed(2).replace('.', ',')}
                    </p>
                  </div>
                  <div className="flex justify-between items-center">
                    <span className="text-gray-400 text-sm">Quantidade:</span>
                    <p className="text-white text-lg font-semibold">
                      {produto.quantidade} unidades
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
                  onClick={() => handleExcluir(produto.id)}
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
              <Label htmlFor="edit-nome" className="text-gray-200">Nome do Produto</Label>
              <Input
                id="edit-nome"
                value={nome}
                onChange={(e) => setNome(e.target.value)}
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
                    src={previewImage.startsWith('data:') ? previewImage : `${API_IMG}${previewImage}`}
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
