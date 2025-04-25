import { useState, useEffect } from "react";
import { VendaService } from "@/services/api";
import { Venda } from "@/lib/types";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
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
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { toast } from "sonner";
import { Search, Download, Filter, Calendar, User, DollarSign } from "lucide-react";
import { format, isValid, parseISO } from "date-fns";
import { ptBR } from "date-fns/locale";
import { useQuery } from "@tanstack/react-query";
import { useAuth } from "@/contexts/AuthContext";

interface VendasPorFuncionario {
  usuarioId: string;
  usuarioNome: string;
  totalVendas: number;
  totalValor: number;
  vendas: Venda[];
}

interface ProcessedVendas {
  vendasFiltradas: Venda[];
  vendasPorFuncionario: VendasPorFuncionario[];
}

export default function HistoricoVendas() {
  const [filtro, setFiltro] = useState("");
  const [dataInicio, setDataInicio] = useState("");
  const [dataFim, setDataFim] = useState("");
  const [formaPagamento, setFormaPagamento] = useState<string>("todos");
  const { user } = useAuth();

  const { data: vendas, isLoading, error } = useQuery({
    queryKey: ['vendas'],
    queryFn: VendaService.getVendas,
    retry: false
  });

  useEffect(() => {
    if (error) {
      toast.error(error.message || "Erro ao carregar histórico de vendas");
    }
  }, [error]);

  const formatarData = (dataString: string) => {
    try {
      const data = parseISO(dataString);
      if (!isValid(data)) {
        return "Data inválida";
      }
      return format(data, "dd/MM/yyyy HH:mm", { locale: ptBR });
    } catch (error) {
      return "Data inválida";
    }
  };

  const processarVendas = (): ProcessedVendas => {
    if (!vendas) return { vendasFiltradas: [], vendasPorFuncionario: [] };

    // Se for admin, os dados já vêm agrupados
    if (user?.role === 'admin') {
      const vendasPorFuncionario = vendas as unknown as VendasPorFuncionario[];
      
      // Aplicar filtros em cada grupo de vendas
      const vendasPorFuncionarioFiltradas = vendasPorFuncionario.map(funcionario => {
        let vendasFiltradas = [...funcionario.vendas];

        // Filtrar por texto (cliente, ID ou usuário)
        if (filtro) {
          vendasFiltradas = vendasFiltradas.filter(venda => 
            (venda.cliente?.toLowerCase() || '').includes(filtro.toLowerCase()) ||
            (venda.id?.toLowerCase() || '').includes(filtro.toLowerCase()) ||
            (venda.usuarioNome?.toLowerCase() || '').includes(filtro.toLowerCase())
          );
        }

        // Filtrar por data
        if (dataInicio) {
          const dataInicioObj = parseISO(dataInicio);
          if (isValid(dataInicioObj)) {
            vendasFiltradas = vendasFiltradas.filter(venda => {
              const dataVenda = parseISO(venda.data);
              return isValid(dataVenda) && dataVenda >= dataInicioObj;
            });
          }
        }
        if (dataFim) {
          const dataFimObj = parseISO(dataFim);
          if (isValid(dataFimObj)) {
            vendasFiltradas = vendasFiltradas.filter(venda => {
              const dataVenda = parseISO(venda.data);
              return isValid(dataVenda) && dataVenda <= dataFimObj;
            });
          }
        }

        // Filtrar por forma de pagamento
        if (formaPagamento && formaPagamento !== "todos") {
          vendasFiltradas = vendasFiltradas.filter(venda => 
            venda.formaPagamento === formaPagamento
          );
        }

        // Recalcular totais
        const totalVendas = vendasFiltradas.length;
        const totalValor = vendasFiltradas.reduce((total, venda) => total + (venda.total || 0), 0);

        return {
          ...funcionario,
          vendas: vendasFiltradas,
          totalVendas,
          totalValor
        };
      });

      // Juntar todas as vendas filtradas
      const todasVendasFiltradas = vendasPorFuncionarioFiltradas.flatMap(funcionario => funcionario.vendas);

      return { 
        vendasFiltradas: todasVendasFiltradas, 
        vendasPorFuncionario: vendasPorFuncionarioFiltradas 
      };
    }

    // Se não for admin, os dados vêm como lista de vendas
    const vendasLista = vendas as Venda[];
    let vendasFiltradas = [...vendasLista];

    // Filtrar por texto (cliente, ID ou usuário)
    if (filtro) {
      vendasFiltradas = vendasFiltradas.filter(venda => 
        (venda.cliente?.toLowerCase() || '').includes(filtro.toLowerCase()) ||
        (venda.id?.toLowerCase() || '').includes(filtro.toLowerCase()) ||
        (venda.usuarioNome?.toLowerCase() || '').includes(filtro.toLowerCase())
      );
    }

    // Filtrar por data
    if (dataInicio) {
      const dataInicioObj = parseISO(dataInicio);
      if (isValid(dataInicioObj)) {
        vendasFiltradas = vendasFiltradas.filter(venda => {
          const dataVenda = parseISO(venda.data);
          return isValid(dataVenda) && dataVenda >= dataInicioObj;
        });
      }
    }
    if (dataFim) {
      const dataFimObj = parseISO(dataFim);
      if (isValid(dataFimObj)) {
        vendasFiltradas = vendasFiltradas.filter(venda => {
          const dataVenda = parseISO(venda.data);
          return isValid(dataVenda) && dataVenda <= dataFimObj;
        });
      }
    }

    // Filtrar por forma de pagamento
    if (formaPagamento && formaPagamento !== "todos") {
      vendasFiltradas = vendasFiltradas.filter(venda => 
        venda.formaPagamento === formaPagamento
      );
    }

    // Para não-admin, criar um único grupo com as vendas do usuário
    const vendasPorFuncionario = [{
      usuarioId: user?.id || '',
      usuarioNome: user?.email || '',
      totalVendas: vendasFiltradas.length,
      totalValor: vendasFiltradas.reduce((total, venda) => total + (venda.total || 0), 0),
      vendas: vendasFiltradas
    }];

    return { vendasFiltradas, vendasPorFuncionario };
  };

  const { vendasFiltradas, vendasPorFuncionario } = processarVendas();

  const exportarRelatorio = () => {
    // Criar conteúdo do relatório
    let conteudo = "Relatório de Vendas\n\n";
    conteudo += `Período: ${dataInicio ? formatarData(dataInicio) : "Todo"} - ${dataFim ? formatarData(dataFim) : "Todo"}\n\n`;
    
    // Adicionar resumo por funcionário
    conteudo += "Resumo por Funcionário:\n";
    vendasPorFuncionario.forEach(funcionario => {
      conteudo += `\n${funcionario.usuarioNome}:\n`;
      conteudo += `Total de Vendas: ${funcionario.totalVendas}\n`;
      conteudo += `Valor Total: R$ ${funcionario.totalValor.toFixed(2)}\n`;
      conteudo += `Ticket Médio: R$ ${(funcionario.totalValor / funcionario.totalVendas || 0).toFixed(2)}\n`;
      conteudo += "-------------------\n";
    });
    
    // Adicionar resumo geral
    const totalVendas = vendasFiltradas.length;
    const valorTotal = vendasFiltradas.reduce((total, venda) => total + (venda.total || 0), 0);
    const mediaTicket = totalVendas > 0 ? valorTotal / totalVendas : 0;
    
    conteudo += `\nResumo Geral:\n`;
    conteudo += `Total de Vendas: ${totalVendas}\n`;
    conteudo += `Valor Total: R$ ${valorTotal.toFixed(2)}\n`;
    conteudo += `Ticket Médio: R$ ${mediaTicket.toFixed(2)}\n\n`;
    
    // Adicionar detalhes das vendas
    conteudo += "Detalhes das Vendas:\n";
    vendasFiltradas.forEach(venda => {
      conteudo += `\nVenda ID: ${venda.id}\n`;
      conteudo += `Data: ${formatarData(venda.data)}\n`;
      conteudo += `Cliente: ${venda.cliente || 'Não informado'}\n`;
      conteudo += `Vendedor: ${venda.usuarioNome}\n`;
      conteudo += `Forma de Pagamento: ${venda.formaPagamento}\n`;
      conteudo += `Total: R$ ${(venda.total || 0).toFixed(2)}\n`;
      conteudo += "Itens:\n";
      venda.itens?.forEach(item => {
        conteudo += `- ${item.quantidade}x ${item.produto?.nome || 'Produto não encontrado'} = R$ ${((item.produto?.preco || 0) * item.quantidade).toFixed(2)}\n`;
      });
      conteudo += "-------------------\n";
    });
    
    // Criar e baixar arquivo
    const blob = new Blob([conteudo], { type: "text/plain" });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `relatorio-vendas-${format(new Date(), "yyyy-MM-dd")}.txt`;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
  };

  const formasPagamento = [
    { valor: "pix", label: "PIX" },
    { valor: "credito", label: "Cartão de Crédito" },
    { valor: "debito", label: "Cartão de Débito" },
    { valor: "dinheiro", label: "Dinheiro" },
  ];

  const formatarVenda = (venda: Venda) => {
    let conteudo = `Venda #${venda.id}\n`;
    conteudo += `Data: ${new Date(venda.data).toLocaleString()}\n`;
    conteudo += `Cliente: ${venda.cliente}\n`;
    conteudo += `Forma de Pagamento: ${venda.formaPagamento}\n`;
    conteudo += `Total: R$ ${venda.total.toFixed(2)}\n\n`;
    conteudo += 'Itens:\n';
    venda.itens.forEach(item => {
      conteudo += `- ${item.quantidade}x ${item.produto?.nome || 'Produto não encontrado'} = R$ ${(item.precoUnitario * item.quantidade).toFixed(2)}\n`;
    });
    return conteudo;
  };

  if (error) {
    return (
      <div className="flex items-center justify-center h-full">
        <div className="text-center">
          <h3 className="text-xl font-semibold text-red-500 mb-2">Erro ao carregar vendas</h3>
          <p className="text-gray-400">{error.message}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-3xl font-semibold tracking-tight text-white font-['Poppins']">Histórico de Vendas</h2>
          <p className="text-muted-foreground">Acompanhe todas as vendas realizadas.</p>
        </div>
        
        <Button
          className="bg-viber-gold hover:bg-viber-gold/80 text-black"
          onClick={exportarRelatorio}
        >
          <Download className="h-4 w-4 mr-2" />
          Exportar Relatório
        </Button>
      </div>

      {user?.role === 'admin' && (
        <Card className="bg-zinc-800 border-zinc-700">
          <CardHeader>
            <CardTitle className="text-white">Filtros</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
              <div className="space-y-2">
                <Label htmlFor="filtro" className="text-gray-200">Buscar</Label>
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
                  <Input
                    id="filtro"
                    placeholder="Cliente, ID ou Vendedor"
                    value={filtro}
                    onChange={(e) => setFiltro(e.target.value)}
                    className="bg-zinc-700 border-zinc-600 text-white pl-9"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <Label htmlFor="dataInicio" className="text-gray-200">Data Início</Label>
                <Input
                  id="dataInicio"
                  type="date"
                  value={dataInicio}
                  onChange={(e) => setDataInicio(e.target.value)}
                  className="bg-zinc-700 border-zinc-600 text-white"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="dataFim" className="text-gray-200">Data Fim</Label>
                <Input
                  id="dataFim"
                  type="date"
                  value={dataFim}
                  onChange={(e) => setDataFim(e.target.value)}
                  className="bg-zinc-700 border-zinc-600 text-white"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="formaPagamento" className="text-gray-200">Forma de Pagamento</Label>
                <Select value={formaPagamento} onValueChange={setFormaPagamento}>
                  <SelectTrigger className="bg-zinc-700 border-zinc-600 text-white">
                    <SelectValue placeholder="Todas as formas" />
                  </SelectTrigger>
                  <SelectContent className="bg-zinc-800 border-zinc-700">
                    <SelectItem value="todos" className="text-white hover:bg-zinc-700">
                      Todas as formas
                    </SelectItem>
                    {formasPagamento.map(forma => (
                      <SelectItem
                        key={forma.valor}
                        value={forma.valor}
                        className="text-white hover:bg-zinc-700"
                      >
                        {forma.label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>
          </CardContent>
        </Card>
      )}

      {user?.role === 'admin' && (
        <Card className="bg-zinc-800 border-zinc-700">
          <CardHeader>
            <CardTitle className="text-white">Resumo por Funcionário</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {vendasPorFuncionario.map(funcionario => (
                <div key={funcionario.usuarioId} className="bg-zinc-700 p-4 rounded-lg">
                  <div className="flex items-center gap-2 mb-2">
                    <User className="h-5 w-5 text-viber-gold" />
                    <h3 className="text-lg font-semibold text-white">{funcionario.usuarioNome}</h3>
                  </div>
                  <div className="space-y-1">
                    <p className="text-gray-300">
                      <span className="text-gray-400">Total de Vendas:</span> {funcionario.totalVendas}
                    </p>
                    <p className="text-gray-300">
                      <span className="text-gray-400">Valor Total:</span> R$ {funcionario.totalValor.toFixed(2)}
                    </p>
                    <p className="text-gray-300">
                      <span className="text-gray-400">Ticket Médio:</span> R$ {(funcionario.totalValor / funcionario.totalVendas || 0).toFixed(2)}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      )}

      <Card className="bg-zinc-800 border-zinc-700">
        <CardContent className="p-0">
          {isLoading ? (
            <div className="p-8 text-center">
              <p className="text-gray-400">Carregando vendas...</p>
            </div>
          ) : vendasFiltradas.length === 0 ? (
            <div className="p-8 text-center">
              <p className="text-gray-400">Nenhuma venda encontrada com os filtros selecionados.</p>
            </div>
          ) : (
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow className="border-zinc-700">
                    <TableHead className="text-gray-200">ID</TableHead>
                    <TableHead className="text-gray-200">Data</TableHead>
                    <TableHead className="text-gray-200">Cliente</TableHead>
                    <TableHead className="text-gray-200">Vendedor</TableHead>
                    <TableHead className="text-gray-200">Forma de Pagamento</TableHead>
                    <TableHead className="text-gray-200">Total</TableHead>
                    <TableHead className="text-gray-200">Status</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {vendasFiltradas.map((venda) => (
                    <TableRow key={venda.id} className="border-zinc-700">
                      <TableCell className="text-gray-300">{venda.id}</TableCell>
                      <TableCell className="text-gray-300">
                        {formatarData(venda.data)}
                      </TableCell>
                      <TableCell className="text-gray-300">{venda.cliente || 'Não informado'}</TableCell>
                      <TableCell className="text-gray-300">
                        <div className="flex items-center gap-2">
                          <User className="h-4 w-4 text-gray-400" />
                          {venda.usuarioNome}
                        </div>
                      </TableCell>
                      <TableCell className="text-gray-300">
                        {formasPagamento.find(f => f.valor === venda.formaPagamento)?.label || venda.formaPagamento}
                      </TableCell>
                      <TableCell className="text-gray-300">
                        R$ {(venda.total || 0).toFixed(2)}
                      </TableCell>
                      <TableCell className="text-gray-300">
                        <span className={`px-2 py-1 rounded-full text-xs ${
                          venda.status === 'ativa' 
                            ? 'bg-green-500/20 text-green-400' 
                            : 'bg-red-500/20 text-red-400'
                        }`}>
                          {venda.status === 'ativa' ? 'Ativa' : 'Cancelada'}
                        </span>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
} 