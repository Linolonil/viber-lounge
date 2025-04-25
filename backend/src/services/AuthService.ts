import bcrypt from 'bcryptjs';
import jwt from 'jsonwebtoken';
import { Usuario, UsuarioLogin, UsuarioResponse } from '../types/Usuario';
import { readFileSync, writeFileSync, existsSync, mkdirSync } from 'fs';
import { join } from 'path';

const JWT_SECRET = process.env.JWT_SECRET || 'your-secret-key';
const DATA_DIR = join(__dirname, '../../data');
const USERS_FILE = join(DATA_DIR, 'usuarios.json');

export class AuthService {
  private static initializeDataFile(): void {
    if (!existsSync(DATA_DIR)) {
      mkdirSync(DATA_DIR, { recursive: true });
    }

    if (!existsSync(USERS_FILE)) {
      // Criar usuário admin padrão
      const adminUser: Usuario = {
        id: '1',
        nome: 'Administrador',
        email: 'admin@admin.com',
        senha: bcrypt.hashSync('admin123', 10),
        role: 'admin'
      };
      writeFileSync(USERS_FILE, JSON.stringify([adminUser], null, 2));
    }
  }

  private static getUsers(): Usuario[] {
    this.initializeDataFile();
    const data = readFileSync(USERS_FILE, 'utf-8');
    return JSON.parse(data);
  }

  private static saveUsers(users: Usuario[]): void {
    writeFileSync(USERS_FILE, JSON.stringify(users, null, 2));
  }

  static async login(credentials: UsuarioLogin): Promise<{ token: string; user: UsuarioResponse }> {
    const users = this.getUsers();
    const user = users.find(u => u.email === credentials.email);

    if (!user) {
      throw new Error('Usuário não encontrado');
    }

    const isValidPassword = await bcrypt.compare(credentials.senha, user.senha);
    if (!isValidPassword) {
      throw new Error('Senha incorreta');
    }

    const token = jwt.sign(
      { id: user.id, email: user.email, role: user.role },
      JWT_SECRET,
      { expiresIn: '24h' }
    );

    const { senha, ...userWithoutPassword } = user;
    return { token, user: userWithoutPassword };
  }

  static async register(userData: Omit<Usuario, 'id'>): Promise<UsuarioResponse> {
    const users = this.getUsers();
    
    if (users.some(u => u.email === userData.email)) {
      throw new Error('Email já cadastrado');
    }

    const hashedPassword = await bcrypt.hash(userData.senha, 10);
    const newUser: Usuario = {
      ...userData,
      id: Math.random().toString(36).substr(2, 9),
      senha: hashedPassword
    };

    users.push(newUser);
    this.saveUsers(users);

    const { senha, ...userWithoutPassword } = newUser;
    return userWithoutPassword;
  }

  static verifyToken(token: string): UsuarioResponse {
    try {
      const decoded = jwt.verify(token, JWT_SECRET) as { id: string; email: string; role: string };
      const users = this.getUsers();
      const user = users.find(u => u.id === decoded.id);

      if (!user) {
        throw new Error('Usuário não encontrado');
      }

      const { senha, ...userWithoutPassword } = user;
      return userWithoutPassword;
    } catch (error) {
      throw new Error('Token inválido');
    }
  }
} 