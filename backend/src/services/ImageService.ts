// backend/src/services/ImageService.ts
import { Request } from 'express';
import multer, { FileFilterCallback } from 'multer';
import sharp from 'sharp';
import path from 'path';
import fs from 'fs';
import { v4 as uuidv4 } from 'uuid';

const uploadDir = path.join(__dirname, '../../uploads/produtos');

// Cria diretório se não existir
if (!fs.existsSync(uploadDir)) {
  fs.mkdirSync(uploadDir, { recursive: true });
}

const storage = multer.diskStorage({
  destination: (req, file, cb) => {
    cb(null, uploadDir);
  },
  filename: (req, file, cb) => {
    const uniqueName = `${uuidv4()}${path.extname(file.originalname)}`;
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
    const filename = `prod-${Date.now()}${path.extname(file.originalname)}`;
    const outputPath = path.join(uploadDir, filename);
    
    await sharp(file.path)
      .resize(800, 800, { fit: 'inside' })
      .jpeg({ quality: 80 })
      .toFile(outputPath);

    // Remove o arquivo original
    fs.unlinkSync(file.path);

    return {
      filename,
      url: `/api/images/produtos/${filename}`
    };
  }

  static getImagePath(filename: string): string {
    return path.join(uploadDir, filename);
  }

  static async deleteImage(filename: string): Promise<void> {
    const imagePath = path.join(uploadDir, filename);
    if (fs.existsSync(imagePath)) {
      fs.unlinkSync(imagePath);
    }
  }
}