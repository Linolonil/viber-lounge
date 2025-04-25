import { IRepository } from '../../core/interfaces/repository.interface';

export abstract class BaseRepository<T> implements IRepository<T> {
  protected abstract table: string;
  protected abstract db: any;

  async findAll(): Promise<T[]> {
    throw new Error('Method not implemented');
  }

  async findById(id: string): Promise<T | null> {
    throw new Error('Method not implemented');
  }

  async create(data: T): Promise<T> {
    throw new Error('Method not implemented');
  }

  async update(id: string, data: Partial<T>): Promise<T> {
    throw new Error('Method not implemented');
  }

  async delete(id: string): Promise<void> {
    throw new Error('Method not implemented');
  }
} 