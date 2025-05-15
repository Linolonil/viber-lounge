import { Router } from 'express';
import multer from 'multer';
import { join } from 'path';
import { deleteImage, uploadImage } from '../controllers/UploadController';

// Configuração do destino e nome do arquivo
const storage = multer.diskStorage({
  destination: (req, file, cb) => {
    cb(null, join(__dirname, '../../uploads'));
  },
  filename: (req, file, cb) => {
    const uniqueSuffix = `${Date.now()}-${file.originalname.replace(/\s+/g, '_')}`;
    cb(null, uniqueSuffix);
  }
});

const upload = multer({ storage });

const router = Router();

// Middleware para tratar erros do multer e do controller
router.post('/upload', (req, res, next) => {
  upload.single('image')(req, res, (err) => {
    if (err) {
      console.error('Erro no upload:', err);
      return res.status(500).json({ message: 'Erro ao fazer upload da imagem.' });
    }

    try {
      uploadImage(req, res);
    } catch (error) {
      console.error('Erro interno:', error);
      return res.status(500).json({ message: 'Erro interno no servidor.' });
    }
  });
});
router.delete('/upload/:filename', deleteImage);

export default router;
