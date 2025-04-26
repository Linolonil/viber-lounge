import React from 'react';
import { Routes, Route } from 'react-router-dom';
import Venda from '../pages/Venda';
import CadastroProduto from '../pages/CadastroProduto';
import ListaProdutos from '../pages/ListaProdutos';
import Dashboard from '../pages/Dashboard';
import HistoricoVendas from '../pages/HistoricoVendas';
import NotFound from '../pages/NotFound';
import Navbar from './Navbar';

const Layout: React.FC = () => {
  return (
    <div className="flex flex-col h-screen">
      <div className="flex flex-1 overflow-hidden">
        <div className="h-full">
          <Navbar />
        </div>
        <main className="flex-1 overflow-auto p-8">
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/venda" element={<Venda />} />
            <Route path="/produtos/cadastro" element={<CadastroProduto />} />
            <Route path="/produtos" element={<ListaProdutos />} />
            <Route path="/dashboard" element={<Dashboard />} />
            <Route path="/historico" element={<HistoricoVendas />} />
            <Route path="*" element={<NotFound />} />
          </Routes>
        </main>
      </div>
    </div>
  );
};

export default Layout;
