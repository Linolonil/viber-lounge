// backend/src/services/ImageService.ts
import { Request } from 'express';
import multer, { FileFilterCallback } from 'multer';
import sharp from 'sharp';
import { existsSync, mkdirSync, unlinkSync } from 'fs';
import { join, extname } from 'path';
import { v4 as uuidv4 } from 'uuid';

const UPLOAD_DIR = join(__dirname, '../../uploads/produtos');

// Cria diretório se não existir
if (!existsSync(UPLOAD_DIR)) {
  mkdirSync(UPLOAD_DIR, { recursive: true });
}

const storage = multer.diskStorage({
  destination: (req, file, cb) => {
    cb(null, UPLOAD_DIR);
  },
  filename: (req, file, cb) => {
    const uniqueName = `${uuidv4()}${extname(file.originalname)}`;
    cb(null, uniqueName);
  }
});

const fileFilter = (
  req: Request,
  file: Express.Multer.File,
  cb: FileFilterCallback
) => {
  if (file.mimetype.startsWith('image/')) {
    cb(null, true);
  } else {
    cb(new Error('Apenas imagens são permitidas!'));
  }
};

export const upload = multer({ 
  storage,
  fileFilter,
  limits: { fileSize: 5 * 1024 * 1024 } // 5MB
});

export class ImageService {
  static async processImage(file: Express.Multer.File): Promise<{ filename: string; url: string }> {
    const filename = `prod-${Date.now()}${extname(file.originalname)}`;
    const outputPath = join(UPLOAD_DIR, filename);
    
    await sharp(file.path)
      .resize(800, 800, { fit: 'inside' })
      .jpeg({ quality: 80 })
      .toFile(outputPath);

    // Remove o arquivo original
    unlinkSync(file.path);

    return {
      filename,
      url: `/api/images/produtos/${filename}`
    };
  }

  static getImagePath(filename: string): string {
    return join(UPLOAD_DIR, filename);
  }

  static async deleteImage(filename: string): Promise<void> {
    const imagePath = join(UPLOAD_DIR, filename);
    if (existsSync(imagePath)) {
      unlinkSync(imagePath);
    }
  }
}