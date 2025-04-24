
import { useState, useEffect } from "react";
import { ProdutoService, VendaService, TraceService, generateId } from "@/services/storage";
import type { Venda } from "@/lib/types";
import { Produto, ItemVenda, FormaPagamento } from "@/lib/types";
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

export default function Venda() {
  const [produtos, setProdutos] = useState<Produto[]>([]);
  const [carrinhoItens, setCarrinhoItens] = useState<ItemVenda[]>([]);
  const [filtro, setFiltro] = useState("");
  const [dialogOpen, setDialogOpen] = useState(false);
  const [cliente, setCliente] = useState("");
  const [formaPagamento, setFormaPagamento] = useState<FormaPagamento | "">("");
  
  useEffect(() => {
    carregarProdutos();
  }, []);

  const carregarProdutos = () => {
    const produtosCarregados = ProdutoService.getAll();
    setProdutos(produtosCarregados);
  };

  const adicionarAoCarrinho = (produto: Produto) => {
    const itemExistente = carrinhoItens.find(item => item.produto.id === produto.id);
    
    if (itemExistente) {
      const novosItens = carrinhoItens.map(item => 
        item.produto.id === produto.id
          ? { ...item, quantidade: item.quantidade + 1 }
          : item
      );
      setCarrinhoItens(novosItens);
    } else {
      setCarrinhoItens([...carrinhoItens, { produto, quantidade: 1 }]);
    }
    
    toast.success(`${produto.nome} adicionado ao carrinho`);
  };

  const alterarQuantidade = (produtoId: string, novaQuantidade: number) => {
    if (novaQuantidade <= 0) {
      removerDoCarrinho(produtoId);
      return;
    }
    
    const novosItens = carrinhoItens.map(item => 
      item.produto.id === produtoId
        ? { ...item, quantidade: novaQuantidade }
        : item
    );
    
    setCarrinhoItens(novosItens);
  };

  const removerDoCarrinho = (produtoId: string) => {
    setCarrinhoItens(carrinhoItens.filter(item => item.produto.id !== produtoId));
  };

  const calcularTotal = () => {
    return carrinhoItens.reduce(
      (total, item) => total + item.produto.preco * item.quantidade,
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

  const confirmarVenda = () => {
    if (!cliente) {
      toast.error("Informe o nome do cliente");
      return;
    }
    
    if (!formaPagamento) {
      toast.error("Selecione uma forma de pagamento");
      return;
    }
    
    const dataVenda = new Date();
    
    const venda: Venda = {
      id: generateId(),
      data: dataVenda.toISOString(),
      itens: [...carrinhoItens],
      cliente,
      formaPagamento: formaPagamento as FormaPagamento,
      total: calcularTotal()
    };
    
    // Salvar venda
    VendaService.save(venda);
    
    // Adicionar entrada no trace
    TraceService.addTraceEntry(venda);
    
    toast.success("Venda realizada com sucesso!");
    
    // Limpar dados
    setDialogOpen(false);
    setCarrinhoItens([]);
    setCliente("");
    setFormaPagamento("");
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
          
          {produtosFiltrados.length === 0 ? (
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
                <Card key={produto.id} className="bg-zinc-800 border-zinc-700 overflow-hidden transition-all hover:shadow-lg hover:shadow-viber-gold/20">
                  <div className="aspect-video w-full overflow-hidden">
                    {produto.imagem ? (
                      <img 
                        src={produto.imagem} 
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
                    <p className="text-viber-gold text-xl font-semibold">
                      R$ {produto.preco.toFixed(2).replace('.', ',')}
                    </p>
                  </CardContent>
                  <CardFooter>
                    <Button 
                      className="w-full bg-viber-gold hover:bg-viber-gold/80 text-black"
                      onClick={() => adicionarAoCarrinho(produto)}
                    >
                      <ShoppingCart className="h-4 w-4 mr-2" />
                      Adicionar
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
                        key={item.produto.id}
                        initial={{ opacity: 0, height: 0 }}
                        animate={{ opacity: 1, height: "auto" }}
                        exit={{ opacity: 0, height: 0 }}
                        transition={{ duration: 0.2 }}
                        className="flex items-center justify-between gap-2 border-b border-zinc-700 pb-4"
                      >
                        <div className="flex items-center gap-3">
                          <div className="h-12 w-12 rounded overflow-hidden">
                            {item.produto.imagem ? (
                              <img 
                                src={item.produto.imagem} 
                                alt={item.produto.nome}
                                className="h-full w-full object-cover"
                              />
                            ) : (
                              <div className="h-full w-full bg-zinc-700 flex items-center justify-center">
                                <span className="text-xs text-gray-400">Sem img</span>
                              </div>
                            )}
                          </div>
                          <div>
                            <p className="text-sm font-medium text-white">{item.produto.nome}</p>
                            <p className="text-xs text-viber-gold">
                              R$ {item.produto.preco.toFixed(2).replace('.', ',')} un
                            </p>
                          </div>
                        </div>
                        
                        <div className="flex items-center gap-2">
                          <div className="flex items-center bg-zinc-700 rounded-md">
                            <Button
                              size="icon"
                              variant="ghost"
                              className="h-7 w-7 rounded-r-none text-white hover:text-viber-gold hover:bg-transparent"
                              onClick={() => alterarQuantidade(item.produto.id, item.quantidade - 1)}
                            >
                              <Minus className="h-4 w-4" />
                            </Button>
                            <span className="w-8 text-center text-sm text-white">
                              {item.quantidade}
                            </span>
                            <Button
                              size="icon"
                              variant="ghost"
                              className="h-7 w-7 rounded-l-none text-white hover:text-viber-gold hover:bg-transparent"
                              onClick={() => alterarQuantidade(item.produto.id, item.quantidade + 1)}
                            >
                              <Plus className="h-4 w-4" />
                            </Button>
                          </div>
                          <Button
                            size="icon"
                            variant="ghost"
                            className="h-7 w-7 text-gray-400 hover:text-red-500 hover:bg-transparent"
                            onClick={() => removerDoCarrinho(item.produto.id)}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                      </motion.div>
                    ))}
                  </AnimatePresence>
                  
                  <div className="pt-4">
                    <div className="flex justify-between text-lg font-semibold">
                      <span className="text-gray-300">Total:</span>
                      <span className="text-viber-gold">
                        R$ {calcularTotal().toFixed(2).replace('.', ',')}
                      </span>
                    </div>
                  </div>
                  
                  <div className="flex gap-2 pt-4">
                    <Button
                      variant="outline"
                      className="flex-1 border-zinc-600 text-gray-200 hover:bg-zinc-700"
                      onClick={limparCarrinho}
                    >
                      <Trash2 className="h-4 w-4 mr-2" />
                      Limpar
                    </Button>
                    <Button 
                      className="flex-1 bg-viber-gold hover:bg-viber-gold/80 text-black"
                      onClick={finalizarVenda}
                    >
                      <Check className="h-4 w-4 mr-2" />
                      Finalizar
                    </Button>
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
                placeholder="Informe o nome do cliente"
                value={cliente}
                onChange={(e) => setCliente(e.target.value)}
                className="bg-zinc-700 border-zinc-600 text-white"
              />
            </div>
            
            <div className="space-y-2">
              <Label htmlFor="pagamento" className="text-gray-200">Forma de Pagamento</Label>
              <Select value={formaPagamento} onValueChange={(value: string) => setFormaPagamento(value as FormaPagamento | "")}>
                <SelectTrigger className="bg-zinc-700 border-zinc-600 text-white">
                  <SelectValue placeholder="Selecione uma forma de pagamento" />
                </SelectTrigger>
                <SelectContent className="bg-zinc-800 border-zinc-700 text-white">
                  {metodosPagamento.map(metodo => (
                    <SelectItem 
                      key={metodo.valor} 
                      value={metodo.valor}
                      className="focus:bg-zinc-700 focus:text-white"
                    >
                      <div className="flex items-center gap-2">
                        <metodo.icone className="h-4 w-4" />
                        <span>{metodo.label}</span>
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            
            <div className="border-t border-zinc-700 mt-4 pt-4">
              <h3 className="text-lg font-semibold mb-3">Resumo do Pedido</h3>
              <div className="space-y-2">
                {carrinhoItens.map(item => (
                  <div key={item.produto.id} className="flex justify-between text-sm">
                    <span>
                      {item.produto.nome} x{item.quantidade}
                    </span>
                    <span className="text-gray-300">
                      R$ {(item.produto.preco * item.quantidade).toFixed(2).replace('.', ',')}
                    </span>
                  </div>
                ))}
                <div className="flex justify-between font-bold text-lg pt-2 border-t border-zinc-700">
                  <span>Total:</span>
                  <span className="text-viber-gold">
                    R$ {calcularTotal().toFixed(2).replace('.', ',')}
                  </span>
                </div>
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
              className="bg-viber-gold hover:bg-viber-gold/80 text-black"
            >
              Confirmar Venda
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
