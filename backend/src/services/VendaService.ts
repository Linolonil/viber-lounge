import { Venda, ItemVenda } from '../types/Venda';
import { VendaRepository } from '../repositories/VendaRepository';
import { generateId } from '../utils/idGenerator';
import { ProdutoService } from './ProdutoService';

export class VendaService {
  private repository: VendaRepository;
  private produtoService: ProdutoService;

  constructor() {
    this.repository = new VendaRepository();
    this.produtoService = new ProdutoService();
  }

  async getAll(): Promise<Venda[]> {
    return this.repository.findAll();
  }

  async getById(id: string): Promise<Venda | undefined> {
    return this.repository.findById(id);
  }

  async create(vendaData: Omit<Venda, 'id' | 'createdAt'>): Promise<Venda> {
    const now = new Date();

    // Validar estoque e preparar itens
    const itensProcessados: ItemVenda[] = [];
    let totalReal = 0;
    const produtosAtualizados = new Map<string, any>();

    try {
      // Validar estrutura dos itens
      if (!vendaData.itens || !Array.isArray(vendaData.itens) || vendaData.itens.length === 0) {
        throw new Error('A venda deve ter pelo menos um item');
      }

      console.log('Dados recebidos:', JSON.stringify(vendaData, null, 2));

      // Primeiro, validar todos os produtos e preparar as atualizações
      for (const item of vendaData.itens) {
        // Validar estrutura do item
        if (!item.produtoId) {
          console.error('Item inválido:', item);
          throw new Error('Cada item deve ter um produtoId válido');
        }

        console.log(`Buscando produto com ID: ${item.produtoId}`);
        const produto = await this.produtoService.getById(item.produtoId);
        console.log('Produto encontrado:', produto);

        if (!produto) {
          throw new Error(`Produto não encontrado: ${item.produtoId}`);
        }

        if (item.quantidade <= 0) {
          throw new Error(`Quantidade inválida para o produto ${produto.nome}`);
        }

        // Validar se a quantidade solicitada não excede o estoque
        if (item.quantidade > produto.quantidade) {
          throw new Error(`Quantidade solicitada (${item.quantidade}) excede o estoque disponível (${produto.quantidade}) para o produto ${produto.nome}`);
        }

        const estoqueAntes = produto.quantidade;
        const estoqueDepois = estoqueAntes - item.quantidade;
        const precoUnitario = produto.preco;
        const subtotal = precoUnitario * item.quantidade;

        itensProcessados.push({
          produtoId: produto.id,
          produtoNome: produto.nome,
          precoUnitario,
          quantidade: item.quantidade,
          subtotal,
          estoqueAntes,
          estoqueDepois
        });

        totalReal += subtotal;

        // Preparar atualização do produto
        const produtoAtualizado = {
          ...produto,
          quantidade: estoqueDepois,
          status: estoqueDepois === 0 ? 'indisponivel' : produto.status
        };
        produtosAtualizados.set(produto.id, produtoAtualizado);
      }

      // Validar se o total informado corresponde ao calculado
      if (Math.abs(totalReal - vendaData.total) > 0.01) {
        throw new Error(`Total informado (${vendaData.total}) não corresponde ao valor real dos itens (${totalReal})`);
      }

      // Criar a venda
      const venda: Venda = {
        ...vendaData,
        id: generateId(),
        createdAt: now.toISOString(),
        status: 'ativa',
        itens: itensProcessados,
        total: totalReal
      };

      // Salvar a venda
      const vendaSalva = await this.repository.save(venda);

      // Se a venda foi salva com sucesso, atualizar os produtos
      for (const [produtoId, produtoAtualizado] of produtosAtualizados) {
        await this.produtoService.update(produtoId, produtoAtualizado);
      }

      return vendaSalva;
    } catch (error) {
      console.error('Erro ao criar venda:', error);
      console.error('Dados recebidos:', JSON.stringify(vendaData, null, 2));
      throw error;
    }
  }

  async cancelSale(id: string, usuarioId: string, usuarioNome: string, motivo: string): Promise<Venda | null> {
    const venda = await this.repository.findById(id);
    if (!venda) return null;

    // Restaurar estoque dos produtos
    for (const item of venda.itens) {
      const produto = await this.produtoService.getById(item.produtoId);
      if (produto) {
        produto.quantidade += item.quantidade;
        if (produto.status === 'indisponivel') {
          produto.status = 'disponivel';
        }
        await this.produtoService.update(produto.id, produto);
      }
    }

    const now = new Date();
    venda.status = 'cancelada';
    venda.canceladaPorId = usuarioId;
    venda.canceladaPorNome = usuarioNome;
    venda.canceladaEm = now.toISOString();
    venda.motivoCancelamento = motivo;

    return this.repository.save(venda);
  }

  async getByDate(date: string): Promise<Venda[]> {
    const vendas = await this.repository.findAll();
    return vendas.filter(v => {
      const vendaDate = new Date(v.createdAt);
      const searchDate = new Date(date);
      return vendaDate.toISOString().split('T')[0] === searchDate.toISOString().split('T')[0];
    });
  }
} 