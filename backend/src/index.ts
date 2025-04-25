import 'reflect-metadata';
import express from 'express';
import cors from 'cors';
import bodyParser from 'body-parser';
import cookieParser from 'cookie-parser';
import produtoRoutes from './routes/produtoRoutes';
import vendaRoutes from './routes/vendaRoutes';
import authRoutes from './routes/authRoutes';
import periodoTrabalhoRoutes from './routes/periodoTrabalhoRoutes';
import { errorHandler } from './middlewares/errorHandler';
import { existsSync, mkdirSync } from 'fs';
import { join } from 'path';
import dotenv from 'dotenv';

// Carrega as variáveis de ambiente
dotenv.config();

const app = express();
const port = process.env.PORT || 3001;

// Configurar diretórios necessários
const uploadsDir = join(__dirname, '../uploads');
const dataDir = join(__dirname, '../data');

if (!existsSync(uploadsDir)) {
  mkdirSync(uploadsDir, { recursive: true });
}

if (!existsSync(dataDir)) {
  mkdirSync(dataDir, { recursive: true });
}

// Middlewares
app.use(cors({
  origin: ['http://localhost:8080', 'http://localhost:5173'],
  credentials: true,
  methods: ['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS'],
  allowedHeaders: ['Content-Type', 'Authorization']
}));
app.use(cookieParser());
app.use(bodyParser.json({ limit: process.env.MAX_FILE_SIZE || '50mb' }));
app.use(bodyParser.urlencoded({ limit: process.env.MAX_FILE_SIZE || '50mb', extended: true }));

// Routes
app.use('/api/images', express.static(process.env.UPLOAD_DIR || 'uploads'));
app.use('/api/auth', authRoutes);
app.use('/api', produtoRoutes);
app.use('/api', vendaRoutes);
app.use('/api/periodo-trabalho', periodoTrabalhoRoutes);

// Error handling middleware
app.use(errorHandler);

app.listen(port, () => {
  console.log(`Server running at http://localhost:${port}`);
}); 