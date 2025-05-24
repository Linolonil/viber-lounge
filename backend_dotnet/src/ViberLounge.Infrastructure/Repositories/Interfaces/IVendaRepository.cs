using ViberLounge.Domain.Entities;

namespace ViberLounge.Infrastructure.Repositories.Interfaces
{
    public interface IVendaRepository
    {
        Task<Venda?> GetSaleByIdAsync(int id);
        Task<VendaItem?> GetSaleItemByIdAsync(int id);
        Task<List<Venda>> GetSalesByDateAsync(DateTime startDate, DateTime endDate);
        Task<Venda?> CancelSaleAsync(Venda sale, List<VendaCancelada> cancelamentos, List<Produto> products);
        Task<List<VendaItem>> CancelSaleItemAsync(List<VendaItem> item, List<VendaCancelada> cancelamento, List<Produto> product);
        Task<Venda?> CreateSaleWithItemsAndUpdateProductsAsync(Venda sale, List<VendaItem> items, List<Produto> products);
    }
}