import React from 'react';
import { Routes, Route } from 'react-router-dom';
import Venda from '../pages/Venda';
import CadastroProduto from '../pages/CadastroProduto';
import ListaProdutos from '../pages/ListaProdutos';
import Dashboard from '../pages/Dashboard';
import HistoricoVendas from '../pages/HistoricoVendas';
import NotFound from '../pages/NotFound';

const Layout: React.FC = () => {
  return (
    
    <Routes>
      <Route path="/" element={<Dashboard />} />
      <Route path="/venda" element={<Venda />} />
      <Route path="/produtos/cadastro" element={<CadastroProduto />} />
      <Route path="/produtos" element={<ListaProdutos />} />
      <Route path="/dashboard" element={<Dashboard />} />
      <Route path="/historico" element={<HistoricoVendas />} />
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
};

export default Layout;
