import React from 'react';
import { motion } from 'framer-motion';
import { Wine } from 'lucide-react';

interface LoginLayoutProps {
  children: React.ReactNode;
}

export const LoginLayout: React.FC<LoginLayoutProps> = ({ children }) => {
  return (
    <div className="min-h-screen flex">
      {/* Left side - Logo and Text */}
      <div className="hidden lg:flex w-1/2 bg-viber-purple items-center justify-center">
        <div className="max-w-md text-center">
          <motion.div
            initial={{ scale: 0.5, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            transition={{ duration: 0.5 }}
            className="flex items-center justify-center mb-8"
          >
            <Wine className="h-16 w-16 text-viber-gold" />
          </motion.div>
          <motion.h1
            initial={{ y: 20, opacity: 0 }}
            animate={{ y: 0, opacity: 1 }}
            transition={{ delay: 0.2, duration: 0.5 }}
            className="text-4xl font-bold text-white mb-4"
          >
            Viber Lounge
          </motion.h1>
          <motion.p
            initial={{ y: 20, opacity: 0 }}
            animate={{ y: 0, opacity: 1 }}
            transition={{ delay: 0.4, duration: 0.5 }}
            className="text-lg text-gray-300"
          >
            Sistema de gerenciamento de vendas e estoque
          </motion.p>
        </div>
      </div>

      {/* Right side - Login Form */}
      <div className="w-full lg:w-1/2 flex items-center justify-center bg-zinc-900">
        <div className="w-full max-w-md px-8">
          {children}
        </div>
      </div>
    </div>
  );
}; 