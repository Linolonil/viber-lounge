import React from 'react';
import { ProductImage } from './ui/product-image';

interface ProductCardProps {
  product: {
    id: string;
    nome: string;
    preco: number;
    imagem?: string | null;
  };
}

export function ProductCard({ product }: ProductCardProps) {
  return (
    <div className="bg-zinc-800 rounded-lg overflow-hidden shadow-lg">
      <div className="aspect-square w-full">
        <ProductImage
          src={product.imagem}
          alt={product.nome}
          className="w-full h-full"
        />
      </div>
      <div className="p-4">
        <h3 className="text-lg font-semibold text-white">{product.nome}</h3>
        <p className="text-viber-gold font-bold">
          R$ {product.preco.toFixed(2)}
        </p>
      </div>
    </div>
  );
} 