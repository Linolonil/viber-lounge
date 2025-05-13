import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../ui/card';
import { toast } from 'sonner';
import { LoginLayout } from './LoginLayout';
import { motion } from 'framer-motion';

export const LoginForm: React.FC = () => {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault(); 
    
    if (!email || !senha) {
      toast.error('Por favor, preencha todos os campos');
      return;
    }

    setIsSubmitting(true);
    
    try {
      
      await login(email, senha);
     
      toast.success('Login realizado com sucesso!');
      navigate('/'); 
    } catch (error) {
      console.error('Erro no login:', error);
      const errorMessage = error?.response?.data?.message || 'Credenciais inválidas';
      toast.error(errorMessage);
      setSenha(''); 
    } finally {
      setIsSubmitting(false);
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
            <CardTitle className="text-2xl font-bold text-white">Bem-vindo</CardTitle>
            <CardDescription className="text-gray-400">
              Entre com suas credenciais para acessar o sistema
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <Input
                  type="email"
                  placeholder="Email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  className="bg-zinc-700/50 border-zinc-600 text-white placeholder:text-gray-400"
                  disabled={isSubmitting}
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
                  disabled={isSubmitting}
                />
              </div>
              <Button 
                disabled={isSubmitting} 
                type="submit" 
                className="w-full bg-viber-gold hover:bg-viber-gold/90 flex justify-center items-center gap-2"
              >
                {isSubmitting ? (
                  <>
                    <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    <span>Carregando...</span>
                  </>
                ) : (
                  "Entrar"
                )}
              </Button>
              <p className="text-center text-sm text-gray-400">
                Não tem uma conta?{' '}
                <Link to="/register" className="text-viber-gold hover:underline">
                  Registre-se
                </Link>
              </p>
            </form>
          </CardContent>
        </Card>
      </motion.div>
    </LoginLayout>
  );
};