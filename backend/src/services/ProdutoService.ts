import { Produto } from '../types/Produto';
import { ProdutoRepository } from '../repositories/ProdutoRepository';
import { generateId } from '../utils/idGenerator';

export class ProdutoService {
  private repository: ProdutoRepository;

  constructor() {
    this.repository = new ProdutoRepository();
  }

  //acessa o repositorio e busca todos os produtos
  async getAll(): Promise<Produto[]> {
    return this.repository.findAll();
  }

  //acessa o repositorio e busca um produto pelo id
  async getById(id: string): Promise<Produto | undefined> {
    return this.repository.findById(id);
  }

  //cria um produto
  async create(produtoData: Omit<Produto, 'id'>): Promise<Produto> {
    const produto: Produto = {
      ...produtoData,
      id: generateId()
    };
    return this.repository.save(produto);
  }

  //atualiza um produto
  async update(id: string, produtoData: Omit<Produto, 'id'>): Promise<Produto | undefined> {
    const existingProduto = await this.repository.findById(id);
    if (!existingProduto) {
      return undefined;
    }

    const produto: Produto = {
      ...produtoData,
      id: existingProduto.id
    };
    return this.repository.save(produto);
  }

  //deleta um produto
  async delete(id: string): Promise<boolean> {
    const produto = await this.repository.findById(id);
    if (!produto) {
      return false;
    }
    await this.repository.delete(id);
    return true;
  }
} 