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

        public async Task<Produto> GetByIdAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id) ?? throw new KeyNotFoundException($"Produto with ID {id} not found.");
            return produto;
        }

        public async Task<IEnumerable<Produto>> GetAllAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task AddAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var produto = await GetByIdAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
        }
    }
}
