import { Link, useLocation, useNavigate } from "react-router-dom";
import { LayoutDashboard, ShoppingCart, Package, Plus, History, LogOut, User, Clock } from "lucide-react";
import { useAuth } from "../contexts/AuthContext";
import { toast } from "sonner";

export default function Navbar() {
  const location = useLocation();
  const navigate = useNavigate();
  const { logout, user } = useAuth();

  const menuItems = [
    { path: "/dashboard", icon: LayoutDashboard, label: "Dashboard", adminOnly: false },
    { path: "/venda", icon: ShoppingCart, label: "Nova Venda", adminOnly: false },
    { path: "/produtos", icon: Package, label: "Produtos", adminOnly: true },
    { path: "/produtos/cadastro", icon: Plus, label: "Cadastrar Produto", adminOnly: true },
    { path: "/historico", icon: History, label: "Histórico de Vendas", adminOnly: false },
  ];

  const handleLogout =  () => {
    try {
      logout();
      navigate('/login');
    } catch (error) {
      toast.error('Erro ao fazer logout');
    }
  };

  // Filtrar itens do menu baseado no papel do usuário
  const filteredMenuItems = menuItems.filter(item => !item.adminOnly || user?.role === 'ADMIN');

  return (
    <nav className="h-full w-64  border-r border-zinc-700 bg-viber-purple p-4 flex flex-col">
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-viber-gold">Viber Lounge</h1>
        {user && (
          <div className="mt-4 p-3 bg-zinc-700/50 rounded-md">
            <div className="flex items-center gap-2 text-gray-300">
              <User className="h-4 w-4" />
              <span className="text-sm">{user.nome}</span>
            </div>
            <div className="text-xs text-gray-400 mt-1">
              {user.role === 'ADMIN' ? 'Administrador' : 'Vendedor'}
            </div>
          </div>
        )}
      </div>
      
      <ul className="space-y-2 flex-1">
        {filteredMenuItems.map(item => (
          <li key={item.path}>
            <Link
              to={item.path}
              className={`flex items-center gap-3 px-4 py-2 rounded-md transition-colors ${
                location.pathname === item.path
                  ? "bg-viber-gold text-black"
                  : "text-gray-300 hover:bg-zinc-700"
              }`}
            >
              <item.icon className="h-5 w-5" />
              {item.label}
            </Link>
          </li>
        ))}
      </ul>

      <div className="space-y-2">
        <button
          className={`w-full flex items-center gap-3 px-4 py-2 rounded-md text-white transition-colors `}
        >
          <Clock className="h-5 w-5" />
        </button>

        <button
          onClick={handleLogout}
          className="w-full flex items-center gap-3 px-4 py-2 rounded-md text-gray-300 hover:bg-zinc-700 transition-colors"
        >
          <LogOut className="h-5 w-5" />
          Sair
        </button>
      </div>
    </nav>
  );
} 