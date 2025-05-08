import { NextFunction, Request, Response } from 'express';
import { ProdutoService } from '../services/ProdutoService';
import { ImageService } from '../services/ImageService';
import { AppError } from '../middlewares/errorHandler';
import { Produto } from '../types/Produto';
import { existsSync, unlinkSync } from 'fs';

export class ProdutoController {
  private produtoService: ProdutoService;

  constructor() {
    this.produtoService = new ProdutoService();
  }

  //busca todos os produtos
  async getAll(req: Request, res: Response): Promise<void> {
    try {
      const produtos = await this.produtoService.getAll();
      res.json(produtos);
    } catch (error) {
      res.status(500).json({ error: 'Erro ao buscar produtos' });
    }
  }
  //busca um produto pelo id
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
  //cria um produto
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
      console.log(url)
      const produto: Produto = await this.produtoService.create({
        nome,
        preco: precoNumerico,
        imagemUrl: url,
        quantidade: quantidadeNumerica,
        status: 'disponivel'
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
  //atualiza um produto
  async update(req: Request, res: Response, next: NextFunction): Promise<void> {
    try {
      const { id } = req.params;
      const { nome, preco, quantidade } = req.body;

      // Validações (mantidas como antes)
      const quantidadeNumerica = parseInt(quantidade);
      if (isNaN(quantidadeNumerica) || quantidadeNumerica < 0) {
        throw new AppError(400, 'Quantidade inválida');
      }

      const precoNumerico = parseFloat(preco);
      if (isNaN(precoNumerico) || precoNumerico <= 0) {
        throw new AppError(400, 'Preço inválido');
      }

      if (!nome || typeof nome !== 'string' || nome.trim() === '') {
        throw new AppError(400, 'Nome inválido');
      }

      const produtoAtual = await this.produtoService.getById(id);
      if (!produtoAtual) {
        res.status(404).json({ error: 'Produto não encontrado' });
        return;
      }

      const updateData: any = {
        nome: nome.trim(),
        preco: precoNumerico,
        quantidade: quantidadeNumerica,
        imagemUrl: produtoAtual.imagemUrl
      };

      if (req.file) {
        try {
          // 1. Primeiro processa a nova imagem
          const { url } = await ImageService.processImage(req.file);

          // 2. Se a nova imagem foi processada com sucesso, exclui a antiga
          if (produtoAtual.imagemUrl) {
            // Extrai o nome do arquivo da URL
            const oldImagePath = produtoAtual.imagemUrl.split('/').pop();
            if (oldImagePath) {
              await ImageService.deleteImage(oldImagePath);
            }
          }

          // 3. Atualiza a URL da nova imagem
          updateData.imagemUrl = url;

        } catch (error) {
          // Se falhar em qualquer etapa, remove a nova imagem temporária
          if (req.file?.path && existsSync(req.file.path)) {
            unlinkSync(req.file.path);
          }
          throw new AppError(500, 'Falha ao processar imagem. Nenhuma alteração foi feita.');
        }
      }

      // 4. Atualiza o produto no banco de dados
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
  //deleta um produto
  async delete(req: Request, res: Response): Promise<void> {
    try {
      const { id } = req.params;
      const produtoAtual = await this.produtoService.getById(id);

      if (!produtoAtual) {
        res.status(404).json({ error: 'Produto não encontrado' });
        return;
      }

      // Se existir imagem, tenta excluir
      const oldImagePath = produtoAtual.imagemUrl?.split('/').pop();
      if (oldImagePath) {
        await ImageService.deleteImage(oldImagePath);
      }

      // Agora sim exclui os dados do produto
      await this.produtoService.delete(id);

      res.status(204).send("Produto deletado com sucesso");
    } catch (error) {
      res.status(500).json({ error: 'Erro ao deletar produto' });
    }
  }

}
