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
      if (!req.file) {
        throw new AppError(400, 'Imagem do produto é obrigatória');
      }

      const { nome, quantidade, preco } = req.body;
      
      if (!nome || !quantidade || !preco) {
        throw new AppError(400, 'Todos os campos são obrigatórios');
      }

      const precoNumerico = parseFloat(preco);
      const quantidadeNumerica = parseInt(quantidade);

      if (isNaN(precoNumerico) || precoNumerico <= 0) {
        throw new AppError(400, 'Preço inválido');
      }

      if (isNaN(quantidadeNumerica) || quantidadeNumerica < 0) {
        throw new AppError(400, 'Quantidade inválida');
      }

      // Processar a imagem
      const { filename, url } = await ImageService.processImage(req.file);

      const produto: Produto = await this.produtoService.create({
        nome,
        preco: precoNumerico,
        imagemUrl: url,
        quantidade: quantidadeNumerica
      });

      res.status(201).json(produto);
    } catch (error) {
      if (error instanceof AppError) {
        res.status(error.statusCode).json({ error: error.message });
      } else {
        next(error);
      }
    }
  }

  async update(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
      const { id } = req.params;
      const { nome, preco, quantidade } = req.body;

      // Validar quantidade
      const quantidadeNumerica = parseInt(quantidade);
      if (isNaN(quantidadeNumerica) || quantidadeNumerica < 0) {
        throw new AppError(400, 'Quantidade inválida');
      }

      // Validar preço
      const precoNumerico = parseFloat(preco);
      if (isNaN(precoNumerico) || precoNumerico <= 0) {
        throw new AppError(400, 'Preço inválido');
      }

      // Validar nome
      if (!nome || typeof nome !== 'string' || nome.trim() === '') {
        throw new AppError(400, 'Nome inválido');
      }

      // Buscar produto atual
      const produtoAtual = await this.produtoService.getById(id);
      if (!produtoAtual) {
        res.status(404).json({ error: 'Produto não encontrado' });
        return;
      }

      // Preparar dados para atualização
      const updateData: any = { 
        nome: nome.trim(),
        preco: precoNumerico, 
        quantidade: quantidadeNumerica 
      };

      // Se houver nova imagem
      if (req.file) {
        // Excluir imagem antiga
        const filename = produtoAtual.imagemUrl.split('/').pop();
        if (filename) {
          try {
            await ImageService.deleteImage(filename);
          } catch (error) {
            console.error('Erro ao deletar imagem antiga:', error);
          }
        }

        // Processar nova imagem
        const { url } = await ImageService.processImage(req.file);
        updateData.imagemUrl = url;
      } else {
        // Manter a imagem atual
        updateData.imagemUrl = produtoAtual.imagemUrl;
      }

      // Atualizar produto
      const produto = await this.produtoService.update(id, updateData);
      res.json(produto);
    } catch (error) {
      if (error instanceof AppError) {
        res.status(error.statusCode).json({ error: error.message });
      } else {
        next(error);
      }
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

      res.status(204).send();
    } catch (error) {
      res.status(500).json({ error: 'Erro ao deletar produto' });
    }
  }
} 
 