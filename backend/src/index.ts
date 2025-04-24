import express from 'express';
import cors from 'cors';
import bodyParser from 'body-parser';
import produtoRoutes from './routes/produtoRoutes';
import vendaRoutes from './routes/vendaRoutes';
import { errorHandler } from './middlewares/errorHandler';

const app = express();
const port = process.env.PORT || 3000;

// Middlewares
app.use(cors());
app.use(bodyParser.json());

// Routes
app.use("/images", express.static('uploads'))
app.use('/uploads', express.static('uploads'));
app.use('/api', produtoRoutes);
app.use('/api', vendaRoutes);

// Error handling middleware
app.use(errorHandler);

app.listen(port, () => {
  console.log(`Server running at http://localhost:${port}`);
}); 