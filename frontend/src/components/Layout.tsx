
import { Sidebar } from "@/components/ui/sidebar";
import { Link, Outlet, useLocation } from "react-router-dom";
import { Wine, BarChart3, PlusCircle, ShoppingCart, FileEdit } from "lucide-react";
import { cn } from "@/lib/utils";

export default function Layout() {
  const { pathname } = useLocation();

  const navItems = [
    { 
      href: "/", 
      label: "Dashboard", 
      icon: BarChart3,
      active: pathname === "/"
    },
    { 
      href: "/cadastro", 
      label: "Cadastrar Produto", 
      icon: PlusCircle,
      active: pathname === "/cadastro" 
    },
    { 
      href: "/produtos", 
      label: "Editar Produtos", 
      icon: FileEdit,
      active: pathname === "/produtos"
    },
    { 
      href: "/venda", 
      label: "Realizar Venda", 
      icon: ShoppingCart,
      active: pathname === "/venda" 
    },
  ];

  return (
    <div className="flex h-screen bg-black">
      <div className="w-64 bg-viber-purple min-h-screen p-4 flex flex-col shadow-lg">
        <div className="flex items-center gap-2 px-2 mb-8">
          <Wine className="h-8 w-8 text-viber-gold" />
          <h1 className="text-2xl font-bold text-white font-['Poppins']">Viber Lounge</h1>
        </div>

        <nav className="flex-1">
          <ul className="space-y-2">
            {navItems.map((item) => (
              <li key={item.href}>
                <Link
                  to={item.href}
                  className={cn(
                    "flex items-center gap-3 px-3 py-3 rounded-md text-sm transition-colors",
                    item.active 
                      ? "bg-viber-gold/10 text-viber-gold" 
                      : "text-gray-300 hover:text-viber-gold hover:bg-viber-gold/10"
                  )}
                >
                  <item.icon className="h-5 w-5" />
                  <span className="font-medium font-['Inter']">{item.label}</span>
                </Link>
              </li>
            ))}
          </ul>
        </nav>

        <div className="pt-4 pb-2 px-3 mt-auto">
          <div className="bg-viber-gold/10 rounded-md p-3 border border-viber-gold/20">
            <div className="text-viber-gold text-sm font-medium font-['Poppins']">Viber Lounge</div>
            <div className="text-gray-400 text-xs">Sistema de Vendas</div>
          </div>
        </div>
      </div>
      
      <div className="flex-1 overflow-auto bg-zinc-900">
        <main className="p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
