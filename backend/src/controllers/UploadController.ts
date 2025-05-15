import { Request, Response } from 'express';
import { unlink } from 'fs';
import { join } from 'path';

export const uploadImage = (req: Request, res: Response) => {
  try {
    if (!req.file) {
      return res.status(400).json({ message: 'Nenhum arquivo enviado.' });
    }

    const fileUrl = `${req.protocol}://${req.get('host')}/api/images/${req.file.filename}`;
    console.log('File URL:', fileUrl);

    return res.status(201).json({
      message: 'Imagem enviada com sucesso!',
      imageUrl: fileUrl
    });
  } catch (error) {
    console.error('Erro ao fazer upload da imagem:', error);
    return res.status(500).json({ message: 'Erro interno no servidor ao fazer upload da imagem.' });
  }
};

export const deleteImage = (req: Request, res: Response) => {
  try {
    const { filename } = req.params;
    const filePath = join(__dirname, '../../uploads', filename);

    unlink(filePath, (err) => {
      if (err) {
        console.error('Erro ao deletar o arquivo:', err);
        return res.status(404).json({ message: 'Imagem não encontrada ou erro ao deletar.' });
      }

      return res.status(200).json({ message: 'Imagem deletada com sucesso.' });
    });
  } catch (error) {
    console.error('Erro interno:', error);
    return res.status(500).json({ message: 'Erro interno ao tentar deletar a imagem.' });
  }
};
