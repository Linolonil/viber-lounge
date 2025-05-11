using ViberLounge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ViberLounge.Infrastructure.Context;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Infrastructure.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly ApplicationDbContext _context;

        public VendaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateSaleWithItemsAsync(Venda sale, List<VendaItem> items)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Vendas.AddAsync(sale);
                await _context.SaveChangesAsync();

                foreach (var item in items)
                {
                    item.IdVenda = sale.Id;
                }

                await _context.ItensVendas.AddRangeAsync(items);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Erro ao salvar venda e itens: {ex.Message}");
                return false;
            }
        }
    }
}
