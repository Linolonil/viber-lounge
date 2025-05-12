using ViberLounge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ViberLounge.Infrastructure.Context;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Infrastructure.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProdutoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Produto>> GetAllProductAsync(bool includeDeleted = false)
        {
            if (includeDeleted)
            {
                return await _context.Produtos.AsNoTracking()
                    .IgnoreQueryFilters()
                    .ToListAsync();
            }
            return await _context.Produtos.AsNoTracking().Where(p => !p.IsDeleted).ToListAsync();
        }
        public async Task AddAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
        }

        public async Task<Produto> UpdateProductAsync(Produto produto)
        {
            var existingEntity = await _context.Produtos.FindAsync(produto.Id);
            
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            produto.UpdatedAt = DateTime.UtcNow;
            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return await _context.Produtos
                .AsNoTracking()
                .Select(p => new Produto
                {
                    Id = p.Id,
                    Descricao = p.Descricao,
                    DescricaoLonga = p.DescricaoLonga,
                    Preco = p.Preco,
                    ImagemUrl = p.ImagemUrl,
                    Quantidade = p.Quantidade,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    IsDeleted = p.IsDeleted
                })
                .FirstOrDefaultAsync(p => p.Id == produto.Id) ?? produto;
        }

        public async Task<Produto> DeleteProductAsync(Produto produto)
        {
            produto.IsDeleted = true;
            
            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return await _context.Produtos
                .AsNoTracking()
                .Select(p => new Produto
                {
                    Id = p.Id,
                    Descricao = p.Descricao,
                    DescricaoLonga = p.DescricaoLonga,
                    Preco = p.Preco,
                    ImagemUrl = p.ImagemUrl,
                    Quantidade = p.Quantidade,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    IsDeleted = p.IsDeleted
                })
                .FirstOrDefaultAsync(p => p.Id == produto.Id) ?? produto;
        }

        public async Task<Produto?> IsProductExists(string descricao)
        {
            var descNorm = descricao.Trim().ToLowerInvariant();

            return await _context.Produtos
                .FirstOrDefaultAsync(p => 
                    !p.IsDeleted && 
                    p.Descricao!.Trim().ToLower() == descNorm);
        }

        public async Task<Produto?> CreateProductAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public Task<Produto?> GetProductByIdAsync(int id)
        {
            try
            {
                return _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar produto por ID: {ex.Message}");
                return null!;
            }
        }

        public Task<List<Produto>> GetProductsByDescriptionAsync(string descricao)
        {
            var termo = descricao.Trim().ToLowerInvariant();

            return _context.Produtos
                .Where(p => 
                    !p.IsDeleted &&
                    p.Descricao != null &&
                    p.Descricao.Trim().ToLower().Contains(termo))
                .ToListAsync();
        }

        public async Task<Produto> RestoreProductAsync(int id)
        {
            var produto = await _context.Produtos
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted);
                
            if (produto == null)
                throw new KeyNotFoundException($"Produto excluído com ID {id} não encontrado.");
                
            produto.IsDeleted = false;
            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return produto;
        }

        public Task<Produto?> GetProductByIdAndAvailableStatus(int id)
        {
            try
            {
                return _context.Produtos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted && p.Status == "DISPONIVEL");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar produto por ID e status: {ex.Message}");
                return null!;
            }
        }

        public Task<List<Produto>> UpdateProductStockAsync(List<Produto> produtos)
        {
            foreach (var produto in produtos)
            {
                var existingEntity = _context.Produtos.Find(produto.Id);
                
                if (existingEntity != null)
                {
                    existingEntity.Quantidade -= produto.Quantidade;
                    existingEntity.UpdatedAt = DateTime.UtcNow;
                    _context.Entry(existingEntity).State = EntityState.Modified;
                }
            }

            return _context.SaveChangesAsync().ContinueWith(t => produtos);
        }
    }
}
