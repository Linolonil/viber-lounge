import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { LoginForm } from './components/auth/LoginForm';
import { RegisterForm } from './components/auth/RegisterForm';
import { ProtectedRoute } from './components/auth/ProtectedRoute';
import { AdminRoute } from './components/auth/AdminRoute';
import { Toaster } from './components/ui/sonner';
import Layout from './components/Layout';
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import Navbar from "./components/Navbar";
import CadastroProduto from "./pages/CadastroProduto";
import ListaProdutos from "./pages/ListaProdutos";
import Dashboard from "./pages/Dashboard";
import Venda from "./pages/Venda";
import HistoricoVendas from "./pages/HistoricoVendas";

const queryClient = new QueryClient();

function App() {
  return (
    <AuthProvider>
      <QueryClientProvider client={queryClient}>
        <Router>
          <div className="min-h-screen bg-zinc-900 text-white">
            <Routes>
              <Route path="/login" element={<LoginForm />} />
              <Route path="/register" element={<RegisterForm />} />
              
              <Route
                path="/*"
                element={
                  <ProtectedRoute>
                    <div className="flex h-screen">
                      <Navbar />
                      <main className="flex-1 overflow-auto p-8">
                        <Routes>
                          <Route path="/" element={<Dashboard />} />
                          <Route path="/dashboard" element={<Dashboard />} />
                          <Route path="/venda" element={<Venda />} />
                          <Route
                            path="/historico"
                            element={
                              <AdminRoute>
                                <HistoricoVendas />
                              </AdminRoute>
                            }
                          />
                          
                          {/* Rotas administrativas */}
                          <Route
                            path="/produtos"
                            element={
                              <AdminRoute>
                                <ListaProdutos />
                              </AdminRoute>
                            }
                          />
                          <Route
                            path="/produtos/cadastro"
                            element={
                              <AdminRoute>
                                <CadastroProduto />
                              </AdminRoute>
                            }
                          />
                        </Routes>
                      </main>
                    </div>
                  </ProtectedRoute>
                }
              />
            </Routes>
            <Toaster richColors />
          </div>
        </Router>
      </QueryClientProvider>
    </AuthProvider>
  );
}

export default App;
