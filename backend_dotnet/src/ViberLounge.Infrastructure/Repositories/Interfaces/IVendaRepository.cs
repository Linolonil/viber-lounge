using ViberLounge.Domain.Entities;

namespace ViberLounge.Infrastructure.Repositories.Interfaces
{
    public interface IVendaRepository
    {
        Task<bool> CreateSaleWithItemsAsync(Venda sale, List<VendaItem> items);
        Task<bool> CreateSaleWithItemsAndUpdateProductsAsync(Venda sale, List<VendaItem> items, List<Produto> products);
        Task<Venda?> GetSaleByIdAsync(int id);
        Task<bool> CancelSaleAsync(Venda sale, VendaCancelada cancelamento);
        Task<List<Venda>> GetSalesByDateAsync(DateTime startDate, DateTime endDate);
    }
}