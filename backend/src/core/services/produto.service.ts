import { IRepository } from '../interfaces/repository.interface';
import { Produto } from '../entities/produto.entity';

export class ProdutoService {
  constructor(private repository: IRepository<Produto>) {}

  async findAll(): Promise<Produto[]> {
    return this.repository.findAll();
  }

  async findById(id: string): Promise<Produto | null> {
    return this.repository.findById(id);
  }

  async create(data: Produto): Promise<Produto> {
    return this.repository.create(data);
  }

  async update(id: string, data: Partial<Produto>): Promise<Produto> {
    return this.repository.update(id, data);
  }

  async delete(id: string): Promise<void> {
    return this.repository.delete(id);
  }
} 