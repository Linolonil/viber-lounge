import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../ui/card';
import { toast } from 'sonner';
import { LoginLayout } from './LoginLayout';
import { motion } from 'framer-motion';

export const RegisterForm: React.FC = () => {
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [confirmSenha, setConfirmSenha] = useState('');
  const { register } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (senha !== confirmSenha) {
      toast.error('As senhas não coincidem');
      return;
    }

    try {
      await register(nome, email, senha);
      toast.success('Registro realizado com sucesso!');
      navigate('/login');
    } catch (error) {
      toast.error(error.response.data.message);
    }
  };

  return (
    <LoginLayout>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
      >
        <Card className="border-none bg-zinc-800/50">
          <CardHeader className="space-y-1">
            <CardTitle className="text-2xl font-bold text-white">Criar Conta</CardTitle>
            <CardDescription className="text-gray-400">
              Preencha os dados para criar sua conta
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <Input
                  type="text"
                  placeholder="Nome"
                  value={nome}
                  onChange={(e) => setNome(e.target.value)}
                  required
                  className="bg-zinc-700/50 border-zinc-600 text-white placeholder:text-gray-400"
                />
              </div>
              <div className="space-y-2">
                <Input
                  type="email"
                  placeholder="Email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  className="bg-zinc-700/50 border-zinc-600 text-white placeholder:text-gray-400"
                />
              </div>
              <div className="space-y-2">
                <Input
                  type="password"
                  placeholder="Senha"
                  value={senha}
                  onChange={(e) => setSenha(e.target.value)}
                  required
                  className="bg-zinc-700/50 border-zinc-600 text-white placeholder:text-gray-400"
                />
              </div>
              <div className="space-y-2">
                <Input
                  type="password"
                  placeholder="Confirmar Senha"
                  value={confirmSenha}
                  onChange={(e) => setConfirmSenha(e.target.value)}
                  required
                  className="bg-zinc-700/50 border-zinc-600 text-white placeholder:text-gray-400"
                />
              </div>
              <Button type="submit" className="w-full bg-viber-gold hover:bg-viber-gold/90">
                Registrar
              </Button>
              <p className="text-center text-sm text-gray-400">
                Já tem uma conta?{' '}
                <Link to="/login" className="text-viber-gold hover:underline">
                  Faça login
                </Link>
              </p>
            </form>
          </CardContent>
        </Card>
      </motion.div>
    </LoginLayout>
  );
}; 