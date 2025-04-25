import { Request, Response } from 'express';
import { ProdutoService } from '../../core/services/produto.service';
import { RepositoryFactory } from '../../infrastructure/factories/repository.factory';
import { Produto } from '../../core/entities/produto.entity';

export class ProdutoController {
  private service: ProdutoService;

  constructor() {
    const repository = RepositoryFactory.create<Produto>('file');
    this.service = new ProdutoService(repository);
  }

  async findAll(req: Request, res: Response) {
    try {
      const produtos = await this.service.findAll();
      res.json(produtos);
    } catch (error) {
      res.status(500).json({ error: 'Erro ao buscar produtos' });
    }
  }

  async findById(req: Request, res: Response) {
    try {
      const produto = await this.service.findById(req.params.id);
      if (!produto) {
        return res.status(404).json({ error: 'Produto não encontrado' });
      }
      res.json(produto);
    } catch (error) {
      res.status(500).json({ error: 'Erro ao buscar produto' });
    }
  }

  async create(req: Request, res: Response) {
    try {
      const produto = await this.service.create(req.body);
      res.status(201).json(produto);
    } catch (error) {
      res.status(500).json({ error: 'Erro ao criar produto' });
    }
  }

  async update(req: Request, res: Response) {
    try {
      const produto = await this.service.update(req.params.id, req.body);
      res.json(produto);
    } catch (error) {
      res.status(500).json({ error: 'Erro ao atualizar produto' });
    }
  }

  async delete(req: Request, res: Response) {
    try {
      await this.service.delete(req.params.id);
      res.status(204).send();
    } catch (error) {
      res.status(500).json({ error: 'Erro ao deletar produto' });
    }
  }
} 