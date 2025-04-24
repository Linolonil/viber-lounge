import { Produto } from '../types';
import { ProdutoRepository } from '../repositories/ProdutoRepository';
import { generateId } from '../utils/idGenerator';

export class ProdutoService {
  private repository: ProdutoRepository;

  constructor() {
    this.repository = new ProdutoRepository();
  }

  async getAll(): Promise<Produto[]> {
    return this.repository.findAll();
  }

  async getById(id: string): Promise<Produto | undefined> {
    return this.repository.findById(id);
  }

  async create(produtoData: Omit<Produto, 'id'>): Promise<Produto> {
    const produto: Produto = {
      ...produtoData,
      id: generateId()
    };
    return this.repository.save(produto);
  }

  async update(id: string, produtoData: Omit<Produto, 'id'>): Promise<Produto | undefined> {
    const existingProduto = await this.repository.findById(id);
    if (!existingProduto) {
      return undefined;
    }

    const produto: Produto = {
      ...produtoData,
      id
    };
    return this.repository.save(produto);
  }

  async delete(id: string): Promise<boolean> {
    const produto = await this.repository.findById(id);
    if (!produto) {
      return false;
    }
    await this.repository.delete(id);
    return true;
  }
} 