import { Router } from 'express';
import { VendaController } from '../controllers/VendaController';
import { validateVenda } from '../middlewares/vendaValidation';

const router = Router();
const vendaController = new VendaController();

router.get('/vendas', (req, res, next) => vendaController.getAll(req, res, next));
router.get('/vendas/:id', (req, res, next) => vendaController.getById(req, res, next));
router.post('/vendas', validateVenda, (req, res, next) => vendaController.create(req, res, next));
router.get('/vendas/data/:date', (req, res, next) => vendaController.getByDate(req, res, next));
router.delete('/vendas/:id', (req, res, next) => vendaController.cancelSale(req, res, next));
router.delete('/vendas/:id/itens/:itemId', (req, res, next) => vendaController.cancelItem(req, res, next));

// Novos endpoints
router.get('/dashboard/:date', (req, res, next) => vendaController.getDadosByDate(req, res, next));
router.get('/trace/:date', (req, res, next) => vendaController.getTraceByDate(req, res, next));

export default router; 