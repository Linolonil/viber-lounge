import React, { useEffect, useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../ui/card';
import { LoginLayout } from './LoginLayout';
import { motion } from 'framer-motion';

export const LoginForm: React.FC = () => {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const { login, isLoadingLogin, token, user} = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await login(email, senha);
      navigate('/');
    } catch (error) {
      console.log(error)
    } 
  };

    useEffect(() => {
    if (user) {
      navigate("/");
    }
  }, [token, user, navigate]);
   

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
              <Button
                disabled={isLoadingLogin}
                type="submit"
                className={`w-full bg-viber-gold hover:bg-viber-gold/90 disabled:bg-gray-400 disabled:cursor-not-allowed transition-all duration-300 ease-in-out`}
                aria-label="Entrar"
              >
                {isLoadingLogin ? (
                  <div className="flex justify-center items-center">
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      className="w-5 h-5 animate-spin text-white"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      strokeWidth="2"
                    >
                      <circle cx="12" cy="12" r="10" stroke="currentColor" strokeLinecap="round" strokeLinejoin="round"></circle>
                      <path
                        fill="none"
                        stroke="currentColor"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        d="M4 12a8 8 0 0 1 16 0"
                      ></path>
                    </svg>
                  </div>
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