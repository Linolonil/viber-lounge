import { readFileSync, writeFileSync, existsSync, mkdirSync, promises } from 'fs';
import { join } from 'path';
import { Produto } from '../types';

export class ProdutoRepository {
  private readonly dataDir: string;
  private readonly filePath: string;

  constructor() {
    this.dataDir = join(__dirname, '../../data');
    this.filePath = join(this.dataDir, 'produtos.json');
    this.initializeDataFile();
  }

  private initializeDataFile(): void {
    if (!existsSync(this.dataDir)) {
      mkdirSync(this.dataDir, { recursive: true });
    }

    if (!existsSync(this.filePath)) {
      writeFileSync(this.filePath, JSON.stringify([]));
    }
  }

  async findAll(): Promise<Produto[]> {
    const data = await promises.readFile(this.filePath, 'utf-8');
    return JSON.parse(data);
  }

  async findById(id: string): Promise<Produto | undefined> {
    const produtos = await this.findAll();
    return produtos.find(p => p.id === id);
  }

  async save(produto: Produto): Promise<Produto> {
    const produtos = await this.findAll();
    const index = produtos.findIndex(p => p.id === produto.id);

    if (index >= 0) {
      produtos[index] = produto;
    } else {
      produtos.push(produto);
    }

    await promises.writeFile(this.filePath, JSON.stringify(produtos, null, 2));
    return produto;
  }

  async delete(id: string): Promise<void> {
    const produtos = await this.findAll();
    const filteredProdutos = produtos.filter(p => p.id !== id);
    await promises.writeFile(this.filePath, JSON.stringify(filteredProdutos, null, 2));
  }
} 