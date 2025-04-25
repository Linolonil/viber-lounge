import { Request, Response, NextFunction } from 'express';
import { AppError } from '../middlewares/errorHandler';
import { AuthRequest } from '../middlewares/authMiddleware';
import { readFileSync, writeFileSync } from 'fs';
import { join } from 'path';

interface PeriodoTrabalho {
  id: string;
  usuarioId: string;
  usuarioNome: string;
  dataInicio: string;
  dataFim?: string;
  status: 'aberto' | 'fechado';
  totalVendas: number;
  totalValor: number;
}

const PERIODOS_FILE = join(__dirname, '../../data/periodos-trabalho.json');

export class PeriodoTrabalhoController {
  private static getPeriodos(): PeriodoTrabalho[] {
    try {
      const data = readFileSync(PERIODOS_FILE, 'utf-8');
      return JSON.parse(data);
    } catch (error) {
      return [];
    }
  }

  private static savePeriodos(periodos: PeriodoTrabalho[]): void {
    writeFileSync(PERIODOS_FILE, JSON.stringify(periodos, null, 2));
  }

  async getPeriodoAtual(req: AuthRequest, res: Response, next: NextFunction): Promise<void> {
    try {
      if (!req.user) {
        throw new AppError(401, 'Usuário não autenticado');
      }

      const periodos = PeriodoTrabalhoController.getPeriodos();
      const periodoAtual = periodos.find(
        p => p.usuarioId === req.user.id && p.status === 'aberto'
      );

      if (!periodoAtual) {
        res.json(null);
        return;
      }

      res.json(periodoAtual);
    } catch (error) {
      next(new AppError(500, 'Erro ao buscar período de trabalho atual'));
    }
  }

  async iniciarPeriodo(req: AuthRequest, res: Response, next: NextFunction): Promise<void> {
    try {
      if (!req.user) {
        throw new AppError(401, 'Usuário não autenticado');
      }

      const periodos = PeriodoTrabalhoController.getPeriodos();
      const periodoExistente = periodos.find(
        p => p.usuarioId === req.user.id && p.status === 'aberto'
      );

      if (periodoExistente) {
        throw new AppError(400, 'Já existe um período de trabalho em andamento');
      }

      const novoPeriodo: PeriodoTrabalho = {
        id: Math.random().toString(36).substr(2, 9),
        usuarioId: req.user.id,
        usuarioNome: req.user.nome || 'Usuário',
        dataInicio: new Date().toISOString(),
        status: 'aberto',
        totalVendas: 0,
        totalValor: 0
      };

      periodos.push(novoPeriodo);
      PeriodoTrabalhoController.savePeriodos(periodos);

      res.status(201).json(novoPeriodo);
    } catch (error) {
      if (error instanceof AppError) {
        next(error);
        return;
      }
      next(new AppError(500, 'Erro ao iniciar período de trabalho'));
    }
  }

  async encerrarPeriodo(req: AuthRequest, res: Response, next: NextFunction): Promise<void> {
    try {
      if (!req.user) {
        throw new AppError(401, 'Usuário não autenticado');
      }

      const { id } = req.params;
      const periodos = PeriodoTrabalhoController.getPeriodos();
      const periodoIndex = periodos.findIndex(
        p => p.id === id && p.usuarioId === req.user.id
      );

      if (periodoIndex === -1) {
        throw new AppError(404, 'Período de trabalho não encontrado');
      }

      const periodo = periodos[periodoIndex];
      if (periodo.status === 'fechado') {
        throw new AppError(400, 'Período de trabalho já está fechado');
      }

      periodo.status = 'fechado';
      periodo.dataFim = new Date().toISOString();

      PeriodoTrabalhoController.savePeriodos(periodos);

      res.json(periodo);
    } catch (error) {
      if (error instanceof AppError) {
        next(error);
        return;
      }
      next(new AppError(500, 'Erro ao encerrar período de trabalho'));
    }
  }
} 