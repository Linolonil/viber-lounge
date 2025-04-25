import { readFileSync, writeFileSync, existsSync, mkdirSync, promises } from 'fs';
import { join } from 'path';
import { BaseRepository } from './base.repository';

export class FileRepository<T> extends BaseRepository<T> {
  protected table: string;
  protected db: string;

  constructor(fileName: string) {
    super();
    this.table = fileName;
    const dir = join(__dirname, '../../../data');
    if (!existsSync(dir)) {
      mkdirSync(dir, { recursive: true });
    }
    this.db = join(dir, fileName);
    if (!existsSync(this.db)) {
      writeFileSync(this.db, JSON.stringify([]));
    }
  }

  async findAll(): Promise<T[]> {
    const data = await promises.readFile(this.db, 'utf-8');
    return JSON.parse(data);
  }

  async findById(id: string): Promise<T | null> {
    const items = await this.findAll();
    const item = items.find(item => (item as any).id === id);
    return item || null;
  }

  async create(data: T): Promise<T> {
    const items = await this.findAll();
    items.push(data);
    await promises.writeFile(this.db, JSON.stringify(items, null, 2));
    return data;
  }

  async update(id: string, data: Partial<T>): Promise<T> {
    const items = await this.findAll();
    const index = items.findIndex(i => (i as any).id === id);
    if (index === -1) throw new Error('Item não encontrado');
    
    items[index] = { ...items[index], ...data };
    await promises.writeFile(this.db, JSON.stringify(items, null, 2));
    return items[index];
  }

  async delete(id: string): Promise<void> {
    const items = await this.findAll();
    const filtered = items.filter(item => (item as any).id !== id);
    await promises.writeFile(this.db, JSON.stringify(filtered, null, 2));
  }
} 