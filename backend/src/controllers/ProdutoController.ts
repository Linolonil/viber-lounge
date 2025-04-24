import { NextFunction, Request, Response } from 'express';
import { ProdutoService } from '../services/ProdutoService';
import { ImageService } from '../services/ImageService';
import { AppError } from '../middlewares/errorHandler';
import { Produto } from '../types';

export class ProdutoController {
  private produtoService: ProdutoService;

  constructor() {
    this.produtoService = new ProdutoService();
  }

  async getAll(req: Request, res: Response): Promise<void> {
    try {
      const produtos = await this.produtoService.getAll();
      res.json(produtos);
    } catch (error) {
      res.status(500).json({ error: 'Erro ao buscar produtos' });
    }
  }

  async getById(req: Request, res: Response): Promise<void> {
    try {
      const { id } = req.params;
      const produto = await this.produtoService.getById(id);
      
      if (!produto) {
        res.status(404).json({ error: 'Produto não encontrado' });
        return;
      }

      res.json(produto);
    } catch (error) {
      res.status(500).json({ error: 'Erro ao buscar produto' });
    }
  }

  async create(req: Request, res: Response, next: NextFunction): Promise<void> {

    
    try {
      let image_filename = `${req.file?.filename}`;
      const { nome, quantidade, preco } = req.body;
      console.log(req.file);
      
      if (!req.file) {
        throw new AppError(400, 'Imagem do produto é obrigatória');
      }

      // Processa a imagem

      // Cria o produto
      const produto: Produto = await this.produtoService.create({
        nome,
        preco: parseFloat(preco),
        imagemUrl: image_filename,
        quantidade
      });

      res.status(201).json(produto);
    } catch (error) {
      next(error);
    }
  }

  async update(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
      const { id } = req.params;
      const { nome, descricao, preco } = req.body;
      
      const updateData: any = { nome, descricao, preco: parseFloat(preco) };

      // Se uma nova imagem foi enviada
      if (req.file) {
        // Busca o produto atual para deletar a imagem antiga
        const produtoAtual = await this.produtoService.getById(id);
        if (produtoAtual) {
          const filename = produtoAtual.imagemUrl.split('/').pop() || '';
          await ImageService.deleteImage(filename);
        }

        // Processa a nova imagem
        const { url } = await ImageService.processImage(req.file);
        updateData.imagemUrl = url;
      }

      const produto = await this.produtoService.update(id, updateData);
      res.json(produto);
    } catch (error) {
      next(error);
    }
  }

  async delete(req: Request, res: Response): Promise<void> {
    try {
      const { id } = req.params;
      const success = await this.produtoService.delete(id);
      
      if (!success) {
        res.status(404).json({ error: 'Produto não encontrado' });
        return;
      }

      res.status(204).json({ message: 'Produto deletado com sucesso' }) ;
    } catch (error) {
      res.status(500).json({ error: 'Erro ao deletar produto' });
    }
  }
} 