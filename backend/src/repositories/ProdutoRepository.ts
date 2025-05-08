import { writeFileSync, existsSync, mkdirSync, promises } from 'fs';
import { join } from 'path';
import { Produto } from '../types/Produto';

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
//busca todos os produtos do repositorio e retorna um array de produtos
  async findAll(): Promise<Produto[]> {
    const data = await promises.readFile(this.filePath, 'utf-8');
    return JSON.parse(data);
  }
//busca um produto pelo id e retorna um produto
  async findById(id: string): Promise<Produto | undefined> {
    const produtos = await this.findAll();
    return produtos.find(p => p.id === id);
  }
//cria um produto e salva no repositorio se for atualização 
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