import { Venda } from '../types';
import { VendaRepository } from '../repositories/VendaRepository';
import { generateId } from '../utils/idGenerator';

export class VendaService {
  private repository: VendaRepository;

  constructor() {
    this.repository = new VendaRepository();
  }

  async getAll(): Promise<Venda[]> {
    return this.repository.findAll();
  }

  async getById(id: string): Promise<Venda | undefined> {
    return this.repository.findById(id);
  }

  async create(vendaData: Omit<Venda, 'id'>): Promise<Venda> {
    // Validar data
    const dataVenda = new Date(vendaData.data);
    if (isNaN(dataVenda.getTime())) {
      throw new Error('Data inválida');
    }

    // Validar estoque e atualizar
    for (const item of vendaData.itens) {
      if (item.quantidade <= 0) {
        throw new Error(`Quantidade inválida para o produto ${item.produto.nome}`);
      }
      if (item.quantidade > item.produto.quantidade) {
        throw new Error(`Estoque insuficiente para o produto ${item.produto.nome}`);
      }
      // Atualizar estoque
      item.produto.quantidade -= item.quantidade;
    }

    // Calcular total real
    const totalReal = vendaData.itens.reduce((total, item) => {
      return total + (item.produto.preco * item.quantidade);
    }, 0);

    // Validar se o total informado corresponde ao calculado
    if (Math.abs(totalReal - vendaData.total) > 0.01) { // Permitir pequena diferença por causa de arredondamento
      throw new Error('Total informado não corresponde ao valor real dos itens');
    }

    const venda: Venda = {
      ...vendaData,
      id: generateId(),
      status: 'ativa',
      total: totalReal // Usar o total calculado
    };

    return this.repository.save(venda);
  }

  async getByDate(date: string): Promise<Venda[]> {
    const vendas = await this.repository.findAll();
    const startDay = new Date(date);
    startDay.setHours(0, 0, 0, 0);
    
    const endDay = new Date(date);
    endDay.setHours(23, 59, 59, 999);

    
    let vendasFiltradas = vendas.filter(v => {
      const vendaDate = new Date(v.data);
      const resultado = vendaDate >= startDay && vendaDate <= endDay;
      return resultado;
    });

    return vendasFiltradas;
  }

  async cancelSale(id: string): Promise<Venda | undefined> {
    const venda = await this.repository.findById(id);
    
    if (!venda) {
      return undefined;
    }

    // Marcar a venda como cancelada
    venda.status = 'cancelada';
    
    // Atualizar o estoque dos produtos
    for (const item of venda.itens) {
      const produto = item.produto;
      produto.quantidade += item.quantidade; // Devolver a quantidade ao estoque
    }

    return this.repository.save(venda);
  }

  async cancelItem(id: string, itemId: string, quantidade: number): Promise<Venda | undefined> {
    const venda = await this.repository.findById(id);
    
    if (!venda) {
      return undefined;
    }

    // Encontrar o item na venda
    const itemIndex = venda.itens.findIndex(item => item.produto.id === itemId);
    
    if (itemIndex === -1) {
      return undefined;
    }

    const item = venda.itens[itemIndex];

    // Verificar se a quantidade a cancelar é válida
    if (quantidade <= 0 || quantidade > item.quantidade) {
      throw new Error('Quantidade inválida para cancelamento');
    }

    // Devolver a quantidade ao estoque
    item.produto.quantidade += quantidade;

    // Atualizar a quantidade do item
    item.quantidade -= quantidade;

    // Se a quantidade for zero, remover o item
    if (item.quantidade === 0) {
      venda.itens.splice(itemIndex, 1);
    }

    // Recalcular o total da venda
    venda.total = venda.itens.reduce((total, item) => {
      return total + (item.produto.preco * item.quantidade);
    }, 0);

    // Se não houver mais itens, cancelar a venda inteira
    if (venda.itens.length === 0) {
      venda.status = 'cancelada';
    }

    return this.repository.save(venda);
  }
} 