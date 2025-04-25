import { readFileSync, writeFileSync, existsSync, mkdirSync, promises } from 'fs';
import { join } from 'path';
import { Venda } from '../types/Venda';

export class VendaRepository {
  private readonly dataDir: string;
  private readonly filePath: string;

  constructor() {
    this.dataDir = join(__dirname, '../../data');
    this.filePath = join(this.dataDir, 'vendas.json');
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

  async findAll(): Promise<Venda[]> {
    const data = await promises.readFile(this.filePath, 'utf-8');
    return JSON.parse(data);
  }

  async findById(id: string): Promise<Venda | undefined> {
    const vendas = await this.findAll();
    return vendas.find(v => v.id === id);
  }

  async save(venda: Venda): Promise<Venda> {
    const vendas = await this.findAll();
    const index = vendas.findIndex(v => v.id === venda.id);

    if (index >= 0) {
      vendas[index] = venda;
    } else {
      vendas.push(venda);
    }

    await promises.writeFile(this.filePath, JSON.stringify(vendas, null, 2));
    return venda;
  }
} 