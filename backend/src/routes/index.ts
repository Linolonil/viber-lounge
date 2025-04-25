import { Router } from 'express';
import authRoutes from './authRoutes';
import produtoRoutes from './produtoRoutes';
import vendaRoutes from './vendaRoutes';
import periodoTrabalhoRoutes from './periodoTrabalhoRoutes';

const router = Router();

router.use('/auth', authRoutes);
router.use('/produtos', produtoRoutes);
router.use('/vendas', vendaRoutes);
router.use('/periodo-trabalho', periodoTrabalhoRoutes);

export default router; 