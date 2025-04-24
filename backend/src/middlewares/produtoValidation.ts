import { Request, Response, NextFunction } from 'express';
import { Produto } from '../types';

export const validateProduto = (req: Request, res: Response, next: NextFunction): void => {
  const { nome, preco, quantidade } = req.body;
console.log(req.body)
  // Validar nome
  if (!nome || typeof nome !== 'string' || nome.trim() === '') {
    res.status(400).json({ error: 'Nome é obrigatório' });
    return;
  }

  // Validar preço
  if (!preco || isNaN(parseFloat(preco)) || parseFloat(preco) <= 0) {
    res.status(400).json({ error: 'Preço inválido' });
    return;
  }

  // Validar quantidade
  if (!quantidade || isNaN(parseInt(quantidade))) {
    res.status(400).json({ error: 'Quantidade inválida' });
    return;
  }

  next();
};