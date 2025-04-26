import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { API_IMG, ProdutoService, VendaService } from "@/services/api";
import type { Venda } from "@/lib/types";
import { Produto, FormaPagamento } from "@/lib/types";
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { toast } from "sonner";
import { Search, ShoppingCart, Plus, Minus, Trash2, Check, CreditCard, Wallet, Banknote, QrCode } from "lucide-react";
import { AnimatePresence, motion } from "framer-motion";
import { useAuth } from "@/contexts/AuthContext";
import { usePeriodoTrabalho } from "@/contexts/PeriodoTrabalhoContext";

interface CarrinhoItem {
  produtoId: string;
  quantidade: number;
  precoUnitario: number;
  produto?: Produto;
}

export default function Venda() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const { periodoAtual } = usePeriodoTrabalho();
  const [produtos, setProdutos] = useState<Produto[]>([]);
  const [carrinhoItens, setCarrinhoItens] = useState<CarrinhoItem[]>([]);
  const [filtro, setFiltro] = useState("");
  const [dialogOpen, setDialogOpen] = useState(false);
  const [cliente, setCliente] = useState("");
  const [formaPagamento, setFormaPagamento] = useState<FormaPagamento | "">("");
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (!periodoAtual) {
      toast.error("É necessário iniciar um turno para realizar vendas");
      navigate("/dashboard");
      return;
    }
    carregarProdutos();
  }, [periodoAtual, navigate]);

  const carregarProdutos = async () => {
    try {
      setIsLoading(true);
      const produtosCarregados = await ProdutoService.getProdutos();
      setProdutos(produtosCarregados);
    } catch (error) {
      console.error("Erro ao carregar produtos:", error);
      toast.error("Erro ao carregar produtos");
    } finally {
      setIsLoading(false);
    }
  };

  const adicionarAoCarrinho = (produto: Produto) => {
    const itemExistente = carrinhoItens.find(item => item.produtoId === produto.id);

    if (itemExistente) {
      if (itemExistente.quantidade >= produto.quantidade) {
        toast.error('Quantidade indisponível em estoque');
        return;
      }
      setCarrinhoItens(
        carrinhoItens.map(item =>
          item.produtoId === produto.id
            ? { ...item, quantidade: item.quantidade + 1 }
            : item
        )
      );
    } else {
      setCarrinhoItens([
        ...carrinhoItens,
        {
          produtoId: produto.id,
          quantidade: 1,
          precoUnitario: produto.preco,
          produto: produto
        }
      ]);
    }
  };

  const removerDoCarrinho = (produtoId: string) => {
    setCarrinhoItens(carrinhoItens.filter(item => item.produtoId !== produtoId));
  };

  const alterarQuantidade = (produtoId: string, novaQuantidade: number) => {
    if (novaQuantidade < 1) {
      removerDoCarrinho(produtoId);
      return;
    }

    const produto = produtos.find(p => p.id === produtoId);
    if (novaQuantidade > (produto?.quantidade || 0)) {
      toast.error('Quantidade indisponível em estoque');
      return;
    }

    setCarrinhoItens(
      carrinhoItens.map(item =>
        item.produtoId === produtoId
          ? { ...item, quantidade: novaQuantidade }
          : item
      )
    );
  };

  const calcularTotal = () => {
    return carrinhoItens.reduce(
      (total, item) => total + item.precoUnitario * item.quantidade,
      0
    );
  };

  const limparCarrinho = () => {
    setCarrinhoItens([]);
  };

  const finalizarVenda = () => {
    if (carrinhoItens.length === 0) {
      toast.error("Adicione produtos ao carrinho para finalizar a venda");
      return;
    }
    
    setDialogOpen(true);
  };

  const confirmarVenda = async () => {
    if (!periodoAtual) {
      toast.error("É necessário iniciar um turno para realizar vendas");
      navigate("/dashboard");
      return;
    }

    if (!cliente) {
      toast.error("Informe o nome do cliente");
      return;
    }
    
    if (!formaPagamento) {
      toast.error("Selecione uma forma de pagamento");
      return;
    }
    
    try {
      setIsLoading(true);
      
      const venda: Omit<Venda, 'id' | 'createdAt'> = {
        itens: carrinhoItens.map(item => ({
          produtoId: item.produtoId,
          quantidade: item.quantidade,
          precoUnitario: item.precoUnitario,
          produtoNome: item.produto?.nome || '',
          subtotal: item.precoUnitario * item.quantidade,
          estoqueAntes: item.produto?.quantidade || 0,
          estoqueDepois: (item.produto?.quantidade || 0) - item.quantidade
        })),
        cliente,
        formaPagamento: formaPagamento as FormaPagamento,
        total: calcularTotal(),
        status: 'ativa',
        usuarioId: user?.id || '',
        usuarioNome: user?.email || '',
        periodoId: periodoAtual.id,
        terminalId: '1'
      };
      
      await VendaService.createVenda(venda);
      
      toast.success("Venda realizada com sucesso!");
      
      // Limpar dados
      setDialogOpen(false);
      setCarrinhoItens([]);
      setCliente("");
      setFormaPagamento("");
      
      // Recarregar produtos para atualizar estoque
      await carregarProdutos();
    } catch (error) {
      console.error("Erro ao finalizar venda:", error);
      toast.error("Erro ao finalizar venda. Verifique o estoque dos produtos.");
    } finally {
      setIsLoading(false);
    }
  };

  const produtosFiltrados = produtos.filter(p => 
    p.nome.toLowerCase().includes(filtro.toLowerCase())
  );

  const metodosPagamento = [
    { valor: "pix", label: "PIX", icone: QrCode },
    { valor: "credito", label: "Cartão de Crédito", icone: CreditCard },
    { valor: "debito", label: "Cartão de Débito", icone: Wallet },
    { valor: "dinheiro", label: "Dinheiro", icone: Banknote },
  ];

  return (
    <div className="space-y-6">
      <div className="flex flex-col lg:flex-row gap-6">
        {/* Lista de produtos */}
        <div className="lg:w-2/3 space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-3xl font-semibold tracking-tight text-white font-['Poppins']">Realizar Venda</h2>
            
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
          
          {isLoading ? (
            <div className="text-center p-8 bg-zinc-800 rounded-lg border border-zinc-700">
              <p className="text-gray-400">Carregando produtos...</p>
            </div>
          ) : produtosFiltrados.length === 0 ? (
            <div className="text-center p-8 bg-zinc-800 rounded-lg border border-zinc-700">
              <p className="text-gray-400">
                {produtos.length === 0 
                  ? "Nenhum produto cadastrado. Cadastre produtos primeiro." 
                  : "Nenhum produto encontrado com este filtro."}
              </p>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
              {produtosFiltrados.map(produto => (
                <Card key={produto.id} className="bg-zinc-800 border-zinc-700 overflow-hidden transition-all hover:shadow-lg hover:shadow-viber-purple/20 relative">
                  {produto.quantidade === 0 && (
                    <div className="absolute top-0 right-0 bg-red-500 text-white px-3 py-1 text-sm font-medium transform translate-x-2 -translate-y-0 z-10 rounded-full">
                      Esgotado
                    </div>
                  )}
                  <div className="aspect-video w-full overflow-hidden">
                    {produto.imagemUrl ? (
                      <img 
                        src={`${API_IMG}${produto.imagemUrl}`} 
                        alt={produto.nome}
                        className="w-full h-full object-cover"
                      />
                    ) : (
                      <div className="w-full h-full flex items-center justify-center bg-zinc-700">
                        <span className="text-gray-400">Sem imagem</span>
                      </div>
                    )}
                  </div>
                  <CardHeader>
                    <CardTitle className="text-white">{produto.nome}</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="flex justify-between items-center">
                      <p className="text-viber-gold text-xl font-semibold">
                        R$ {produto.preco.toFixed(2).replace('.', ',')}
                      </p>
                      <p className={`text-sm ${produto.quantidade === 0 ? 'text-red-500' : 'text-gray-400'}`}>
                        Estoque: {produto.quantidade}
                      </p>
                    </div>
                  </CardContent>
                  <CardFooter>
                    <Button
                      onClick={() => adicionarAoCarrinho(produto)}
                      disabled={produto.quantidade === 0}
                      className="w-full gap-2 bg-viber-purple hover:bg-viber-purple/80 text-white"
                    >
                      <Plus className="h-4 w-4" />
                      {produto.quantidade === 0 ? 'Produto Esgotado' : 'Adicionar ao Carrinho'}
                    </Button>
                  </CardFooter>
                </Card>
              ))}
            </div>
          )}
        </div>
        
        {/* Carrinho de compras */}
        <div className="lg:w-1/3">
          <Card className="bg-zinc-800 border-zinc-700 sticky top-6">
            <CardHeader className="bg-viber-purple">
              <CardTitle className="text-white flex items-center">
                <ShoppingCart className="mr-2" />
                Carrinho
              </CardTitle>
            </CardHeader>
            <CardContent className="pt-6">
              {carrinhoItens.length === 0 ? (
                <div className="text-center p-8">
                  <p className="text-gray-400">O carrinho está vazio</p>
                </div>
              ) : (
                <div className="space-y-4">
                  <AnimatePresence>
                    {carrinhoItens.map(item => (
                      <motion.div
                        key={item.produtoId}
                        initial={{ opacity: 0, height: 0 }}
                        animate={{ opacity: 1, height: "auto" }}
                        exit={{ opacity: 0, height: 0 }}
                        transition={{ duration: 0.2 }}
                        className="flex items-center justify-between gap-2 border-b border-zinc-700 pb-4"
                      >
                        <div className="flex items-center gap-3">
                          <div className="h-12 w-12 rounded overflow-hidden">
                            {item.produto?.imagemUrl ? (
                              <img 
                                src={`${API_IMG}${item.produto.imagemUrl}`} 
                                alt={item.produto.nome}
                                className="h-full w-full object-cover"
                              />
                            ) : (
                              <div className="h-full w-full bg-zinc-700 flex items-center justify-center">
                                <span className="text-gray-400 text-xs">Sem imagem</span>
                              </div>
                            )}
                          </div>
                          <div>
                            <p className="text-white font-medium">{item.produto?.nome}</p>
                            <p className="text-viber-gold">
                              R$ {(item.precoUnitario * item.quantidade).toFixed(2).replace('.', ',')}
                            </p>
                          </div>
                        </div>
                        <div className="flex items-center gap-2">
                          <Button
                            variant="outline"
                            size="icon"
                            className="h-8 w-8 border-zinc-600"
                            onClick={() => alterarQuantidade(item.produtoId, item.quantidade - 1)}
                          >
                            <Minus className="h-4 w-4" />
                          </Button>
                          <span className="w-8 text-center text-white">{item.quantidade}</span>
                          <Button
                            variant="outline"
                            size="icon"
                            className="h-8 w-8 border-zinc-600"
                            onClick={() => alterarQuantidade(item.produtoId, item.quantidade + 1)}
                            disabled={item.quantidade >= (item.produto?.quantidade || 0)}
                          >
                            <Plus className="h-4 w-4" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-8 w-8 text-red-500 hover:text-red-600 hover:bg-red-500/10"
                            onClick={() => removerDoCarrinho(item.produtoId)}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                      </motion.div>
                    ))}
                  </AnimatePresence>
                  
                  <div className="pt-4 border-t border-zinc-700">
                    <div className="flex justify-between items-center mb-4">
                      <span className="text-gray-400">Total:</span>
                      <span className="text-viber-purple text-xl font-semibold">
                        R$ {calcularTotal().toFixed(2).replace('.', ',')}
                      </span>
                    </div>
                    
                    <div className="flex gap-2">
                      <Button
                        variant="outline"
                        className="flex-1 border-zinc-600 text-gray-200 hover:bg-zinc-700 text-black"
                        onClick={limparCarrinho}
                      >
                        Limpar
                      </Button>
                      <Button
                        className="flex-1 bg-viber-purple hover:bg-viber-purple/80 text-white"
                        onClick={finalizarVenda}
                      >
                        Finalizar Venda
                      </Button>
                    </div>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>
        </div>
      </div>
      
      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent className="bg-zinc-800 text-white border-zinc-700">
          <DialogHeader>
            <DialogTitle>Finalizar Venda</DialogTitle>
          </DialogHeader>
          
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <Label htmlFor="cliente" className="text-gray-200">Nome do Cliente</Label>
              <Input
                id="cliente"
                value={cliente}
                onChange={(e) => setCliente(e.target.value)}
                className="bg-zinc-700 border-zinc-600 text-white"
                placeholder="Digite o nome do cliente"
              />
            </div>
            
            <div className="space-y-2">
              <Label htmlFor="formaPagamento" className="text-gray-200">Forma de Pagamento</Label>
              <Select value={formaPagamento} onValueChange={(value) => setFormaPagamento(value as FormaPagamento)}>
                <SelectTrigger className="bg-zinc-700 border-zinc-600 text-white">
                  <SelectValue placeholder="Selecione a forma de pagamento" />
                </SelectTrigger>
                <SelectContent className="bg-zinc-800 border-zinc-700">
                  {metodosPagamento.map(metodo => (
                    <SelectItem 
                      key={metodo.valor} 
                      value={metodo.valor}
                      className="text-white hover:bg-zinc-700"
                    >
                      <div className="flex items-center gap-2">
                        <metodo.icone className="h-4 w-4" />
                        {metodo.label}
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            
            <div className="pt-4 border-t border-zinc-700">
              <div className="flex justify-between items-center mb-4">
                <span className="text-gray-400">Total da Venda:</span>
                <span className="text-viber-purple text-xl font-semibold">
                  R$ {calcularTotal().toFixed(2).replace('.', ',')}
                </span>
              </div>
            </div>
          </div>
          
          <DialogFooter>
            <Button 
              variant="outline" 
              onClick={() => setDialogOpen(false)}
              className="border-zinc-600 text-gray-200 hover:bg-zinc-700"
            >
              Cancelar
            </Button>
            <Button 
              onClick={confirmarVenda}
              className="bg-viber-purple hover:bg-viber-purple/80 text-white"
              disabled={isLoading}
            >
              {isLoading ? "Processando..." : "Confirmar Venda"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
