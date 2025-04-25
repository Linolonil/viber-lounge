import { Router } from 'express';
import { VendaController } from '../controllers/VendaController';
import { validateVenda } from '../middlewares/vendaValidation';
import { authMiddleware } from '../middlewares/authMiddleware';

const router = Router();
const vendaController = new VendaController();

// Todas as rotas de venda requerem autenticação
router.use('/vendas', authMiddleware);

router.get('/vendas', vendaController.getAll.bind(vendaController));
router.get('/vendas/:id', vendaController.getById.bind(vendaController));
router.post('/vendas', validateVenda, vendaController.create.bind(vendaController));
router.get('/vendas/data/:date', vendaController.getByDate.bind(vendaController));
router.delete('/vendas/:id', vendaController.cancelSale.bind(vendaController));
router.delete('/vendas/:id/itens/:itemId', vendaController.cancelItem.bind(vendaController));

// Endpoints de relatórios
router.get('/dashboard/:date', vendaController.getDadosByDate.bind(vendaController));
router.get('/trace/:date', vendaController.getTraceByDate.bind(vendaController));

export default router; 