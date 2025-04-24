import fs from 'fs';
import path from 'path';
import { Produto, Venda } from '../types';

const DATA_DIR = path.join(__dirname, '../../data');
const PRODUTOS_FILE = path.join(DATA_DIR, 'produtos.json');
const VENDAS_FILE = path.join(DATA_DIR, 'vendas.json');

// Ensure data directory exists
if (!fs.existsSync(DATA_DIR)) {
  fs.mkdirSync(DATA_DIR, { recursive: true });
}

// Initialize files if they don't exist
if (!fs.existsSync(PRODUTOS_FILE)) {
  fs.writeFileSync(PRODUTOS_FILE, JSON.stringify([]));
}

if (!fs.existsSync(VENDAS_FILE)) {
  fs.writeFileSync(VENDAS_FILE, JSON.stringify([]));
}

export const ProdutoService = {
  getAll: (): Produto[] => {
    const data = fs.readFileSync(PRODUTOS_FILE, 'utf-8');
    return JSON.parse(data);
  },
  
  getById: (id: string): Produto | undefined => {
    const produtos = ProdutoService.getAll();
    return produtos.find(p => p.id === id);
  },
  
  save: (produto: Produto): void => {
    const produtos = ProdutoService.getAll();
    const index = produtos.findIndex(p => p.id === produto.id);
    
    if (index >= 0) {
      produtos[index] = produto;
    } else {
      produtos.push(produto);
    }
    
    fs.writeFileSync(PRODUTOS_FILE, JSON.stringify(produtos, null, 2));
  },
  
  delete: (id: string): void => {
    const produtos = ProdutoService.getAll().filter(p => p.id !== id);
    fs.writeFileSync(PRODUTOS_FILE, JSON.stringify(produtos, null, 2));
  }
};

export const VendaService = {
  getAll: (): Venda[] => {
    const data = fs.readFileSync(VENDAS_FILE, 'utf-8');
    return JSON.parse(data);
  },
  
  getById: (id: string): Venda | undefined => {
    const vendas = VendaService.getAll();
    return vendas.find(v => v.id === id);
  },
  
  save: (venda: Venda): void => {
    const vendas = VendaService.getAll();
    const index = vendas.findIndex(v => v.id === venda.id);
    
    if (index >= 0) {
      vendas[index] = venda;
    } else {
      vendas.push(venda);
    }
    
    fs.writeFileSync(VENDAS_FILE, JSON.stringify(vendas, null, 2));
  },
  
  getByDate: (date: string): Venda[] => {
    const vendas = VendaService.getAll();
    const startDay = new Date(date);
    startDay.setHours(0, 0, 0, 0);
    
    const endDay = new Date(date);
    endDay.setHours(23, 59, 59, 999);
    
    return vendas.filter(v => {
      const vendaDate = new Date(v.data);
      return vendaDate >= startDay && vendaDate <= endDay;
    });
  }
};

export const generateId = (): string => {
  return Date.now().toString(36) + Math.random().toString(36).substring(2);
}; 