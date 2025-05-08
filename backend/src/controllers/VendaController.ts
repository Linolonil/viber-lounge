import { Request, Response, NextFunction } from 'express';
import { VendaService } from '../services/VendaService';
import { AppError } from '../middlewares/errorHandler';
import { readFileSync, writeFileSync } from 'fs';
import { join } from 'path';
import { Venda, ItemVenda } from '../types/Venda';
import { AuthRequest } from '../middlewares/authMiddleware';
import { PeriodoTrabalho, VendasPorUsuario } from '../interfaces/VendaInterface';
import { DadosGrafico } from '../types/Produto';


const VENDAS_FILE = join(__dirname, '../../data/vendas.json');
const PERIODOS_FILE = join(__dirname, '../../data/periodos-trabalho.json');


export class VendaController {
  private vendaService: VendaService;

  constructor(vendaService?: VendaService) {
    this.vendaService = vendaService || new VendaService();
  }

  private getVendas(): Venda[] {
    try {
      const data = readFileSync(VENDAS_FILE, 'utf-8');
      return JSON.parse(data);
    } catch (error) {
      return [];
    }
  }

  private saveVendas(vendas: Venda[]): void {
    writeFileSync(VENDAS_FILE, JSON.stringify(vendas, null, 2));
  }

  private getPeriodos(): PeriodoTrabalho[] {
    try {
      const data = readFileSync(PERIODOS_FILE, 'utf-8');
      return JSON.parse(data);
    } catch (error) {
      return [];
    }
  }

  private savePeriodos(periodos: PeriodoTrabalho[]): void {
    writeFileSync(PERIODOS_FILE, JSON.stringify(periodos, null, 2));
  }

  private atualizarPeriodoTrabalho(venda: Venda): void {
    const periodos = this.getPeriodos();
    const periodoAtual = periodos.find(
      p => p.id === venda.periodoId && p.status === 'aberto'
    );

    if (periodoAtual) {
      periodoAtual.totalVendas += 1;
      periodoAtual.totalValor += venda.total;
      this.savePeriodos(periodos);
    }
  }

  async getAll(req: AuthRequest, res: Response, next: NextFunction): Promise<void> {
    try {
      const user = req.user;
      if (!user) {
        next(new AppError(401, 'Usuário não autenticado'));
        return;
      }

      const vendas = this.getVendas();
      
      // Se for admin, retorna vendas agrupadas por usuário
      if (user.role === 'admin') {
        const vendasPorUsuario = new Map<string, VendasPorUsuario>();

        vendas.forEach(venda => {
          const usuarioId = venda.usuarioId;
          if (!vendasPorUsuario.has(usuarioId)) {
            vendasPorUsuario.set(usuarioId, {
              usuarioId: venda.usuarioId,
              usuarioNome: venda.usuarioNome,
              totalVendas: 0,
              totalValor: 0,
              vendas: []
            });
          }

          const usuarioVendas = vendasPorUsuario.get(usuarioId)!;
          usuarioVendas.totalVendas += 1;
          usuarioVendas.totalValor += venda.total;
          usuarioVendas.vendas.push(venda);
        });

        res.json(Array.from(vendasPorUsuario.values()));
        return;
      }

      // Se não for admin, retorna apenas as vendas do usuário
      const vendasFiltradas = vendas.filter(v => v.usuarioId === user.id);
      res.json(vendasFiltradas);
    } catch (error) {
      next(new AppError(500, 'Erro ao buscar vendas'));
    }
  }

  async getById(req: AuthRequest, res: Response, next: NextFunction): Promise<void> {
    try {
      const user = req.user;
      if (!user) {
        next(new AppError(401, 'Usuário não autenticado'));
        return;
      }

      const { id } = req.params;
      const vendas = this.getVendas();
      const venda = vendas.find((v: Venda) => v.id === id);
      
      if (!venda) {
        next(new AppError(404, 'Venda não encontrada'));
        return;
      }

      // Verifica se o usuário tem permissão para ver a venda
      if (user.role !== 'admin' && venda.usuarioId !== user.id) {
        next(new AppError(403, 'Você não tem permissão para ver esta venda'));
        return;
      }

      res.json(venda);
    } catch (error) {
      next(new AppError(500, 'Erro ao buscar venda'));
    }
  }

  async create(req: AuthRequest, res: Response, next: NextFunction): Promise<void> {
    try {
      const user = req.user;
      if (!user) {
        next(new AppError(401, 'Usuário não autenticado'));
        return;
      }

      // Verificar se existe um período de trabalho ativo para este usuário específico
      const periodos = this.getPeriodos();
      const periodoAtual = periodos.find(
        p => p.usuarioId === user.id && p.status === 'aberto'
      );

      if (!periodoAtual) {
        next(new AppError(400, 'É necessário iniciar um período de trabalho antes de realizar vendas'));
        return;
      }

      const vendaData: Omit<Venda, 'id' | 'createdAt'> = {
        ...req.body,
        usuarioId: user.id,
        usuarioNome: user.nome || user.email,
        periodoId: periodoAtual.id,
        status: 'ativa',
        terminalId: req.body.terminalId || '1'
      };

      // Usar o VendaService para criar a venda
      const novaVenda = await this.vendaService.create(vendaData);

      // Atualiza o período de trabalho específico deste usuário
      this.atualizarPeriodoTrabalho(novaVenda);

      res.status(201).json(novaVenda);
    } catch (error) {
      console.error('Erro detalhado:', error);
      
      if (error instanceof Error) {
        // Erros de validação de estoque
        if (error.message.includes('Quantidade solicitada') || 
            error.message.includes('excede o estoque disponível')) {
          next(new AppError(400, error.message));
          return;
        }
        // Erros de validação de produto
        if (error.message.includes('Produto não encontrado')) {
          next(new AppError(400, error.message));
          return;
        }
        // Erros de validação de quantidade
        if (error.message.includes('Quantidade inválida')) {
          next(new AppError(400, error.message));
          return;
        }
        // Erros de validação de total
        if (error.message.includes('Total informado')) {
          next(new AppError(400, error.message));
          return;
        }
        // Se for outro erro, retorna a mensagem específica
        next(new AppError(500, `Erro ao criar venda: ${error.message}`));
        return;
      }
      
      next(new AppError(500, 'Erro ao criar venda'));
    }
  }

  async getByDate(req: AuthRequest, res: Response, next: NextFunction): Promise<void> {
    try {
      const user = req.user;
      if (!user) {
        next(new AppError(401, 'Usuário não autenticado'));
        return;
      }

      const { date } = req.params;
      
      if (!/^\d{4}-\d{2}-\d{2}$/.test(date)) {
        next(new AppError(400, 'Formato de data inválido. Use YYYY-MM-DD'));
        return;
      }

      const vendas = await this.vendaService.getByDate(date);
      
      // Se for admin, retorna vendas agrupadas por usuário
      if (user.role === 'admin') {
        const vendasPorUsuario = new Map<string, VendasPorUsuario>();

        vendas.forEach(venda => {
          const usuarioId = venda.usuarioId;
          if (!vendasPorUsuario.has(usuarioId)) {
            vendasPorUsuario.set(usuarioId, {
              usuarioId: venda.usuarioId,
              usuarioNome: venda.usuarioNome,
              totalVendas: 0,
              totalValor: 0,
              vendas: []
            });
          }

          const usuarioVendas = vendasPorUsuario.get(usuarioId)!;
          usuarioVendas.totalVendas += 1;
          usuarioVendas.totalValor += venda.total;
          usuarioVendas.vendas.push(venda);
        });

        res.json(Array.from(vendasPorUsuario.values()));
        return;
      }

      // Se não for admin, retorna apenas as vendas do usuário
      const vendasFiltradas = vendas.filter(v => v.usuarioId === user.id);
      res.json(vendasFiltradas);
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
        .filter(venda => venda.status !== 'cancelada')
        .map(venda => {
          const vendaDate = new Date(venda.createdAt);
          const header = `Venda ID: ${venda.id} | Data: ${vendaDate.toLocaleString()} | Cliente: ${venda.cliente || 'Não informado'}`;
          const payment = `Pagamento: ${venda.formaPagamento.toUpperCase()} | Total: R$ ${venda.total.toFixed(2)}`;
          
          const items = venda.itens.map(item => 
            `- ${item.quantidade}x ${item.produtoNome} (${item.produtoId}) = R$ ${item.subtotal.toFixed(2)}`
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

        venda.itens.forEach(item => {
          const produtoNoMapa = produtosMap.get(item.produtoId);

          if (produtoNoMapa) {
            produtoNoMapa.quantidade += item.quantidade;
          } else {
            produtosMap.set(item.produtoId, {
              id: item.produtoId,
              nome: item.produtoNome,
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
  async cancelSale(req: AuthRequest, res: Response, next: NextFunction): Promise<void> {
    try {
      const user = req.user;
      if (!user) {
        next(new AppError(401, 'Usuário não autenticado'));
        return;
      }

      const { id } = req.params;
      const { motivo } = req.body;

      if (!motivo) {
        next(new AppError(400, 'Motivo do cancelamento é obrigatório'));
        return;
      }

      const venda = await this.vendaService.cancelSale(
        id,
        user.id,
        user.nome || user.email,
        motivo
      );
      
      if (!venda) {
        next(new AppError(404, 'Venda não encontrada'));
        return;
      }

      res.json({ message: 'Venda cancelada com sucesso', venda });
    } catch (error) {
      next(new AppError(500, 'Erro ao cancelar venda'));
    }
  }
}