
import { Produto, Venda } from "@/lib/types";

// Chaves para o localStorage
const PRODUTOS_KEY = 'viber_produtos';
const VENDAS_KEY = 'viber_vendas';
const TRACE_KEY_PREFIX = 'viber_trace_';

// Serviço para produtos
export const ProdutoService = {
  getAll: (): Produto[] => {
    const data = localStorage.getItem(PRODUTOS_KEY);
    return data ? JSON.parse(data) : [];
  },
  
  getById: (id: string): Produto | undefined => {
    const produtos = ProdutoService.getAll();
    return produtos.find(p => p.id === id);
  },
  
  save: (produto: Produto): void => {
    const produtos = ProdutoService.getAll();
    const index = produtos.findIndex(p => p.id === produto.id);
    
    if (index >= 0) {
      produtos[index] = produto;
    } else {
      produtos.push(produto);
    }
    
    localStorage.setItem(PRODUTOS_KEY, JSON.stringify(produtos));
  },
  
  delete: (id: string): void => {
    const produtos = ProdutoService.getAll().filter(p => p.id !== id);
    localStorage.setItem(PRODUTOS_KEY, JSON.stringify(produtos));
  }
};

// Serviço para vendas
export const VendaService = {
  getAll: (): Venda[] => {
    const data = localStorage.getItem(VENDAS_KEY);
    return data ? JSON.parse(data) : [];
  },
  
  getById: (id: string): Venda | undefined => {
    const vendas = VendaService.getAll();
    return vendas.find(v => v.id === id);
  },
  
  save: (venda: Venda): void => {
    const vendas = VendaService.getAll();
    const index = vendas.findIndex(v => v.id === venda.id);
    
    if (index >= 0) {
      vendas[index] = venda;
    } else {
      vendas.push(venda);
    }
    
    localStorage.setItem(VENDAS_KEY, JSON.stringify(vendas));
  },
  
  getByDate: (date: string): Venda[] => {
    const vendas = VendaService.getAll();
    const startDay = new Date(date);
    startDay.setHours(0, 0, 0, 0);
    
    const endDay = new Date(date);
    endDay.setHours(23, 59, 59, 999);
    
    return vendas.filter(v => {
      const vendaDate = new Date(v.data);
      return vendaDate >= startDay && vendaDate <= endDay;
    });
  }
};

// Serviço para arquivos de trace
export const TraceService = {
  formatTime: (date: Date): string => {
    return date.toTimeString().split(' ')[0];
  },
  
  formatDate: (date: Date): string => {
    return date.toISOString().split('T')[0];
  },
  
  getTraceKey: (date: Date): string => {
    return `${TRACE_KEY_PREFIX}${TraceService.formatDate(date)}`;
  },
  
  addTraceEntry: (venda: Venda): void => {
    const date = new Date(venda.data);
    const traceKey = TraceService.getTraceKey(date);
    
    const existingTrace = localStorage.getItem(traceKey) || '';
    
    // Formatar itens da venda
    const itensFormatados = venda.itens.map(item => 
      `${item.produto.nome} (${item.quantidade})`
    ).join(", ");
    
    // Formatar preço total
    const totalFormatado = venda.total.toFixed(2).replace('.', ',');
    
    // Criar entrada de trace no formato especificado
    const novaEntrada = `[${TraceService.formatTime(date)}] | Cliente: ${venda.cliente} | Bebidas: ${itensFormatados} | Pagamento: ${venda.formaPagamento.charAt(0).toUpperCase() + venda.formaPagamento.slice(1)} | Total: R$ ${totalFormatado}\n`;
    
    // Adicionar ao trace existente
    const traceAtualizado = existingTrace + novaEntrada;
    localStorage.setItem(traceKey, traceAtualizado);
  },
  
  getTraceByDate: (date: Date): string => {
    const traceKey = TraceService.getTraceKey(date);
    return localStorage.getItem(traceKey) || '';
  },
  
  getAllTraceDates: (): string[] => {
    const dates: string[] = [];
    for (let i = 0; i < localStorage.length; i++) {
      const key = localStorage.key(i);
      if (key?.startsWith(TRACE_KEY_PREFIX)) {
        dates.push(key.replace(TRACE_KEY_PREFIX, ''));
      }
    }
    return dates;
  }
};

// Gerar um ID único
export const generateId = (): string => {
  return Date.now().toString(36) + Math.random().toString(36).substring(2);
};

// ...existing code...

export const DashboardService = {
  getDadosByDate: (date: string) => {
    // Exemplo de implementação básica usando VendaService
    const vendas = VendaService.getByDate(date);
    let pix = 0, credito = 0, debito = 0, dinheiro = 0, totalVendas = 0;
    const produtosMap: Record<string, { nome: string, quantidade: number }> = {};

    vendas.forEach(venda => {
      totalVendas += venda.total;
      switch (venda.formaPagamento) {
        case "pix": pix += venda.total; break;
        case "credito": credito += venda.total; break;
        case "debito": debito += venda.total; break;
        case "dinheiro": dinheiro += venda.total; break;
      }
      venda.itens.forEach(item => {
        if (!produtosMap[item.produto.nome]) {
          produtosMap[item.produto.nome] = { nome: item.produto.nome, quantidade: 0 };
        }
        produtosMap[item.produto.nome].quantidade += item.quantidade;
      });
    });

    const produtosMaisVendidos = Object.values(produtosMap).sort((a, b) => b.quantidade - a.quantidade);

    return {
      pix,
      credito,
      debito,
      dinheiro,
      totalVendas,
      produtosMaisVendidos,
    };
  }
};