import { Request, Response, NextFunction } from 'express';
import { Venda } from '../types';

export const validateVenda = (req: Request, res: Response, next: NextFunction): void => {
  const vendaData: Omit<Venda, 'id'> = req.body;

  // Validar itens
  if (!vendaData.itens || !Array.isArray(vendaData.itens) || vendaData.itens.length === 0) {
    res.status(400).json({ error: 'A venda deve ter pelo menos um item' });
    return;
  }

  // Validar cada item
  for (const item of vendaData.itens) {
    if (!item.produto || !item.produto.id) {
      res.status(400).json({ error: 'Cada item deve ter um produto válido' });
      return;
    }
    if (!item.quantidade || item.quantidade <= 0) {
      res.status(400).json({ error: 'Cada item deve ter uma quantidade válida' });
      return;
    }
  }

  // Validar cliente
  if (!vendaData.cliente || typeof vendaData.cliente !== 'string' || vendaData.cliente.trim() === '') {
    res.status(400).json({ error: 'Cliente é obrigatório' });
    return;
  }

  // Validar forma de pagamento
  if (!vendaData.formaPagamento || !['pix', 'credito', 'debito', 'dinheiro'].includes(vendaData.formaPagamento)) {
    res.status(400).json({ error: 'Forma de pagamento inválida' });
    return;
  }

  // Validar total
  if (!vendaData.total || vendaData.total <= 0) {
    res.status(400).json({ error: 'Total deve ser maior que zero' });
    return;
  }

  next();
}; 