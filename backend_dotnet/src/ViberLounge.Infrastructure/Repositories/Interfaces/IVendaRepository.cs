using ViberLounge.Domain.Entities;

namespace ViberLounge.Infrastructure.Repositories.Interfaces
{
    public interface IVendaRepository
    {
        Task<Venda?> GetSaleByIdAsync(int id);
        Task<VendaItem?> GetSaleItemByIdAsync(int id);
        Task<bool> CreateSaleWithItemsAsync(Venda sale, List<VendaItem> items);
        Task<Venda?> CreateSaleWithItemsAndUpdateProductsAsync(Venda sale, List<VendaItem> items, List<Produto> products);
        Task<bool> CancelSaleAsync(Venda sale, VendaCancelada cancelamento);
        Task<List<Venda>> GetSalesByDateAsync(DateTime startDate, DateTime endDate);
    }
}