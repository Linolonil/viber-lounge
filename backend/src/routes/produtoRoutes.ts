import { Router, Request, Response, NextFunction } from 'express';
import { ProdutoController } from '../controllers/ProdutoController';
import { validateProduto } from '../middlewares/produtoValidation';
import multer from 'multer';
import { join, extname } from 'path';
import { existsSync, mkdirSync } from 'fs';

const router = Router();
const produtoController = new ProdutoController();

// Configurar diretório de uploads
const UPLOAD_DIR = join(__dirname, '../../uploads/produtos');
if (!existsSync(UPLOAD_DIR)) {
  mkdirSync(UPLOAD_DIR, { recursive: true });
}

// Configuração do multer
const storage = multer.diskStorage({
  destination: UPLOAD_DIR,
  filename: (req, file, cb) => {
    const uniqueSuffix = Date.now() + '-' + Math.round(Math.random() * 1E9);
    cb(null, uniqueSuffix + extname(file.originalname));
  }
});

// Filtro para aceitar apenas imagens
const fileFilter = (req: Request, file: Express.Multer.File, cb: multer.FileFilterCallback) => {
  const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];
  
  if (allowedTypes.includes(file.mimetype)) {
    cb(null, true);
  } else {
    cb(new Error('Tipo de arquivo não suportado. Apenas imagens são permitidas.'));
  }
};

const upload = multer({
  storage: storage,
  fileFilter: fileFilter,
  limits: {
    fileSize: 10 * 1024 * 1024 // 10MB
  }
});

// Middleware para aceitar tanto 'imagem' quanto 'image'
const uploadMiddleware = (req: Request, res: Response, next: NextFunction) => {
  upload.single('imagem')(req, res, (err) => {
    if (err && err.code === 'LIMIT_UNEXPECTED_FILE') {
      // Se falhar com 'imagem', tenta com 'image'
      upload.single('image')(req, res, next);
    } else {
      next(err);
    }
  });
};

router.get('/produtos', (req, res) => produtoController.getAll(req, res));
router.get('/produtos/:id', (req, res) => produtoController.getById(req, res));
router.post('/produtos', uploadMiddleware, validateProduto, (req, res, next) => produtoController.create(req, res, next));
router.put('/produtos/:id', uploadMiddleware, validateProduto, (req, res, next) => produtoController.update(req, res, next));
router.delete('/produtos/:id', (req, res) => produtoController.delete(req, res));

export default router; 