import { Request, Response } from 'express';
import { AuthService } from '../services/AuthService';
import { UsuarioLogin } from '../types/Usuario';

export class AuthController {
  static async login(req: Request, res: Response) {
    try {
      const credentials: UsuarioLogin = req.body;
      const result = await AuthService.login(credentials);
      res.json(result);
    } catch (error) {
      res.status(401).json({ message: error instanceof Error ? error.message : 'Erro ao fazer login' });
    }
  }

  static async register(req: Request, res: Response) {
    try {
      const userData = req.body;
      const newUser = await AuthService.register(userData);
      res.status(201).json(newUser);
    } catch (error) {
      res.status(400).json({ message: error instanceof Error ? error.message : 'Erro ao registrar usuário' });
    }
  }

  static async logout(req: Request, res: Response) {
    res.json({ message: 'Logout realizado com sucesso' });
  }

  static async me(req: Request, res: Response) {
    try {
      const token = req.headers.authorization?.split(' ')[1];
      if (!token) {
        return res.status(401).json({ message: 'Token não fornecido' });
      }

      const user = await AuthService.verifyToken(token);
      res.json(user);
    } catch (error) {
      res.status(401).json({ message: error instanceof Error ? error.message : 'Token inválido' });
    }
  }
} 