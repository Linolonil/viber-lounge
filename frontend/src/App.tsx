import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { PeriodoTrabalhoProvider } from './contexts/PeriodoTrabalhoContext';
import { LoginForm } from './components/auth/LoginForm';
import { RegisterForm } from './components/auth/RegisterForm';
import { ProtectedRoute } from './components/auth/ProtectedRoute';
import { AdminRoute } from './components/auth/AdminRoute';
import { Toaster } from './components/ui/sonner';
import Layout from './components/Layout';
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";


const queryClient = new QueryClient();

function App() {
  return (
    <AuthProvider>
      <PeriodoTrabalhoProvider>
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
                      <Layout />
                    </ProtectedRoute>
                  }
                />
              </Routes>
              <Toaster 
                richColors 
                position="top-right"
                expand={false}
                closeButton
                theme="light"
                duration={3000}
                style={{
                  background: 'rgba(255, 255, 255, 0.95)',
                  backdropFilter: 'blur(8px)',
                  border: '1px solid rgba(0, 0, 0, 0.1)',
                  borderRadius: '12px',
                  boxShadow: '0 8px 16px -4px rgba(0, 0, 0, 0.1), 0 4px 8px -2px rgba(0, 0, 0, 0.05)'
                }}
                toastOptions={{
                  classNames: {
                    toast: "group toast group-[.toaster]:bg-white group-[.toaster]:text-zinc-900 group-[.toaster]:border-zinc-200 group-[.toaster]:shadow-lg",
                    title: "group-[.toast]:text-zinc-900 group-[.toast]:font-semibold",
                    description: "group-[.toast]:text-zinc-600",
                    actionButton: "group-[.toast]:bg-zinc-100 group-[.toast]:text-zinc-900 group-[.toast]:hover:bg-zinc-200",
                    cancelButton: "group-[.toast]:bg-zinc-100 group-[.toast]:text-zinc-900 group-[.toast]:hover:bg-zinc-200",
                    success: "group-[.toast]:bg-green-50 group-[.toast]:text-green-900 group-[.toast]:border-green-200",
                    error: "group-[.toast]:bg-red-50 group-[.toast]:text-red-900 group-[.toast]:border-red-200",
                    warning: "group-[.toast]:bg-yellow-50 group-[.toast]:text-yellow-900 group-[.toast]:border-yellow-200",
                    info: "group-[.toast]:bg-blue-50 group-[.toast]:text-blue-900 group-[.toast]:border-blue-200",
                  },
                }}
              />
            </div>
          </Router>
        </QueryClientProvider>
      </PeriodoTrabalhoProvider>
    </AuthProvider>
  );
}

export default App;
