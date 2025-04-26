import React from 'react';
import { cn } from '@/lib/utils';

interface ProductImageProps extends React.ImgHTMLAttributes<HTMLImageElement> {
  src?: string | null;
  alt: string;
  className?: string;
}

export function ProductImage({ src, alt, className, ...props }: ProductImageProps) {
  const [error, setError] = React.useState(false);

  const defaultImage = '/images/no-product.png'; // Imagem padrão

  const handleError = () => {
    setError(true);
  };

  return (
    <img
      src={error || !src ? defaultImage : src}
      alt={alt}
      onError={handleError}
      className={cn(
        'object-cover rounded-lg',
        className
      )}
      {...props}
    />
  );
} 