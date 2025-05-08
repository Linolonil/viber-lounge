import { Router } from 'express';
import { ProdutoController } from '../controllers/ProdutoController';
import { validateProduto } from '../middlewares/produtoValidation';

import { produtoUploadMiddleware  } from '../config/multerConfig';

const router = Router();
const produtoController = new ProdutoController();
const uploadMiddleware = produtoUploadMiddleware;

//rota para buscar todos os produtos
router.get('/produtos', (req, res) => produtoController.getAll(req, res));
//rota para buscar um produto pelo id
router.get('/produtos/:id', (req, res) => produtoController.getById(req, res));
//rota para criar um produto
router.post('/produtos', uploadMiddleware, validateProduto, (req, res, next) => produtoController.create(req, res, next));
//rota para atualizar um produto
router.put('/produtos/:id', uploadMiddleware, validateProduto, (req, res, next) => produtoController.update(req, res, next));
//rota para deletar um produto
router.delete('/produtos/:id', (req, res) => produtoController.delete(req, res));
export default router; 