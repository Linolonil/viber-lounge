import { IRepository } from '../../core/interfaces/repository.interface';
import { FileRepository } from '../repositories/file.repository';
import { Produto } from '../../core/entities/produto.entity';

export class RepositoryFactory {
  static create<T>(type: string): IRepository<T> {
    switch (type) {
      case 'file':
        return new FileRepository() as unknown as IRepository<T>;
      default:
        throw new Error(`Repository type ${type} not supported`);
    }
  }
} 