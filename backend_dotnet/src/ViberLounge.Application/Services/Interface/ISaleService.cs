using ViberLounge.Application.DTOs.Sale;
using ViberLounge.Domain.Entities;

namespace ViberLounge.Application.Services.Interfaces
{
    public interface ISaleService
    {
        Task<SaleResponseDto> CreateSaleAsync(CreateSaleDto saleDto);
        Task<SaleResponseDto?> GetSaleByIdAsync(int id);
        Task<Venda?> CancelSaleAsync(int saleId, CancelEntireSaleDto sale);
        Task<List<VendaItem>> CancelSaleItemAsync(int saleId, CancelSaleItemsDto cancelSaleItems);
        Task<List<SaleResponseFromDataDto>> GetSalesByDateAsync(SaleRequestFromDataDto saleRequest);
    }
}
