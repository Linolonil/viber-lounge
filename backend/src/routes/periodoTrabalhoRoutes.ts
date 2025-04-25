import { Router } from 'express';
import { PeriodoTrabalhoController } from '../controllers/PeriodoTrabalhoController';
import { authMiddleware } from '../middlewares/authMiddleware';

const router = Router();
const periodoTrabalhoController = new PeriodoTrabalhoController();

router.get('/atual/:usuarioId', authMiddleware, periodoTrabalhoController.getPeriodoAtual);
router.post('/iniciar', authMiddleware, periodoTrabalhoController.iniciarPeriodo);
router.post('/encerrar/:id', authMiddleware, periodoTrabalhoController.encerrarPeriodo);

export default router; 