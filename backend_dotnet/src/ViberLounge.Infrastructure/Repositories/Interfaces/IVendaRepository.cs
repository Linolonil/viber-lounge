using ViberLounge.Domain.Entities;

namespace ViberLounge.Infrastructure.Repositories.Interfaces
{
    public interface IVendaRepository
    {
        Task<bool> CreateSaleWithItemsAsync(Venda sale, List<VendaItem> items);
        // Task<Sale> GetSaleByIdAsync(int id);
        // Task<IEnumerable<Sale>> GetAllSalesAsync();
        // Task<bool> UpdateSaleAsync(Sale sale);
        // Task<bool> DeleteSaleAsync(int id);
    }
}