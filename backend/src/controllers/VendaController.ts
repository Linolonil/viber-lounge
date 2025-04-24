import { Request, Response, NextFunction } from 'express';
import { VendaService } from '../services/VendaService';
import { AppError } from '../middlewares/errorHandler';

interface DadosGrafico {
  pix: number;
  credito: number;
  debito: number;
  dinheiro: number;
  totalVendas: number;
  produtosMaisVendidos: Array<{
    id: string;
    nome: string;
    quantidade: number;
  }>;
}

export class VendaController {
  private vendaService: VendaService;

  constructor(vendaService?: VendaService) {
    this.vendaService = vendaService || new VendaService();
  }

  async getAll(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
      
      const vendas = await this.vendaService.getAll();
      res.json(vendas);
    } catch (error) {
      next(new AppError(500, 'Erro ao buscar vendas'));
    }
  }

  async getById(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
      const { id } = req.params;
      const venda = await this.vendaService.getById(id);
      
      if (!venda) {
        next(new AppError(404, 'Venda não encontrada'));
        return;
      }

      res.json(venda);
    } catch (error) {
      next(new AppError(500, 'Erro ao buscar venda'));
    }
  }

  async create(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
      const vendaData = req.body;
      
      if (!vendaData.itens || vendaData.itens.length === 0) {
        next(new AppError(400, 'A venda deve ter pelo menos um item'));
        return;
      }

      const venda = await this.vendaService.create(vendaData);
      res.status(201).json(venda);
    } catch (error) {
      if (error instanceof Error) {
        if (error.message.includes('Data inválida') || 
            error.message.includes('Quantidade inválida') || 
            error.message.includes('Estoque insuficiente') ||
            error.message.includes('Total informado')) {
          next(new AppError(400, error.message));
          return;
        }
      }
      next(new AppError(500, 'Erro ao criar venda'));
    }
  }

  async getByDate(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
      const { date } = req.params;
      
      if (!/^\d{4}-\d{2}-\d{2}$/.test(date)) {
        next(new AppError(400, 'Formato de data inválido. Use YYYY-MM-DD'));
        return;
      }

      const vendas = await this.vendaService.getByDate(date);
      res.json(vendas);
    } catch (error) {
      next(new AppError(500, 'Erro ao buscar vendas por data'));
    }
  }

  async getTraceByDate(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
        const { date } = req.params;

        // Validação da data
        if (!/^\d{4}-\d{2}-\d{2}$/.test(date)) {
            throw new AppError(400, 'Formato de data inválido. Use YYYY-MM-DD');
        }

        // Buscar vendas para a data especificada
        const vendas = await this.vendaService.getByDate(date);

        // Gerar log detalhado
        const traceContent = vendas
            .filter(venda => venda.status !== 'cancelada') // Ignorar vendas canceladas
            .map(venda => {
                const header = `Venda ID: ${venda.id} | Data: ${new Date(venda.data).toLocaleString()} | Cliente: ${venda.cliente || 'Não informado'}`;
                const payment = `Pagamento: ${venda.formaPagamento.toUpperCase()} | Total: R$ ${venda.total.toFixed(2)}`;
                
                const items = venda.itens.map(item => 
                    `- ${item.quantidade}x ${item.produto.nome} (${item.produto.id}) = R$ ${(item.produto.preco * item.quantidade).toFixed(2)}`
                ).join('\n');

                const status = `Status: ${venda.status.toUpperCase()}`;
                const separator = '-'.repeat(60);

                return `${header}\n${payment}\nItens:\n${items}\n${status}\n${separator}`;
            })
            .join('\n\n');

        // Se não houver vendas
        if (!traceContent) {
            res.type('text').send(`Nenhuma venda encontrada para a data ${date}`);
            return;
        }

        // Adicionar cabeçalho com data e total de vendas
        const totalVendas = vendas
            .filter(v => v.status !== 'cancelada')
            .reduce((sum, v) => sum + v.total, 0);
            
        const header = `RELATÓRIO DE VENDAS - ${date}\nTotal de vendas: R$ ${totalVendas.toFixed(2)}\nTotal de pedidos: ${vendas.filter(v => v.status !== 'cancelada').length}\n${'-'.repeat(60)}\n\n`;
        
        res.type('text').send(header + traceContent);
    } catch (error) {
        console.error('Erro ao gerar trace de vendas:', error);
        next(new AppError(500, 'Erro ao gerar relatório de vendas'));
    }
}

  async getDadosByDate(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
      const { date } = req.params;
      
      if (!/^\d{4}-\d{2}-\d{2}$/.test(date)) {
        next(new AppError(400, 'Formato de data inválido. Use YYYY-MM-DD'));
        return;
      }

      const vendas = await this.vendaService.getByDate(date);

      const dadosGrafico: DadosGrafico = {
        pix: 0,
        credito: 0,
        debito: 0,
        dinheiro: 0,
        totalVendas: 0,
        produtosMaisVendidos: [],
      };

      const produtosMap = new Map<string, {id: string, nome: string, quantidade: number}>();

      vendas.forEach(venda => {
        if (venda.status === 'cancelada') return;

        dadosGrafico.totalVendas += venda.total;

        switch (venda.formaPagamento.toLowerCase()) {
          case 'pix':
            dadosGrafico.pix += venda.total;
            break;
          case 'credito':
            dadosGrafico.credito += venda.total;
            break;
          case 'debito':
            dadosGrafico.debito += venda.total;
            break;
          case 'dinheiro':
            dadosGrafico.dinheiro += venda.total;
            break;
        }

        venda.itens?.forEach(item => {
          const produto = item.produto;
          const produtoNoMapa = produtosMap.get(produto.id);

          if (produtoNoMapa) {
            produtoNoMapa.quantidade += item.quantidade;
          } else {
            produtosMap.set(produto.id, {
              id: produto.id,
              nome: produto.nome,
              quantidade: item.quantidade
            });
          }
        });
      });

      dadosGrafico.produtosMaisVendidos = Array.from(produtosMap.values())
        .sort((a, b) => b.quantidade - a.quantidade);

      res.json(dadosGrafico);
    } catch (error) {
      next(new AppError(500, 'Erro ao buscar dados do dashboard'));
    }
  }

  // cancelar venda

  async cancelSale(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
      const { id } = req.params;
      const venda = await this.vendaService.cancelSale(id);
      
      if (!venda) {
        next(new AppError(404, 'Venda não encontrada'));
        return;
      }

      res.json({ message: 'Venda cancelada com sucesso', venda });
    } catch (error) {
      next(new AppError(500, 'Erro ao cancelar venda'));
    }
  }

  async cancelItem(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
      const { id, itemId } = req.params;
      const { quantidade } = req.body;

      if (!quantidade || typeof quantidade !== 'number' || quantidade <= 0) {
        next(new AppError(400, 'Quantidade deve ser um número positivo'));
        return;
      }

      const venda = await this.vendaService.cancelItem(id, itemId, quantidade);
      
      if (!venda) {
        next(new AppError(404, 'Venda ou item não encontrado'));
        return;
      }

      const produto = venda.itens.find(item => item.produto.id === itemId)?.produto;
      const valorCancelado = produto ? quantidade * produto.preco : 0;

      res.json({ 
        message: 'Item cancelado com sucesso', 
        venda,
        valorCancelado
      });
    } catch (error) {
      if (error instanceof Error && error.message === 'Quantidade inválida para cancelamento') {
        next(new AppError(400, error.message));
        return;
      }
      next(new AppError(500, 'Erro ao cancelar item'));
    }
  }
}