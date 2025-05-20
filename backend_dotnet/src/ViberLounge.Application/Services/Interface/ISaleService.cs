using ViberLounge.Application.DTOs.Sale;

namespace ViberLounge.Application.Services.Interfaces
{
    public interface ISaleService
    {
        Task<SaleResponseDto?> CancelSaleAsync(CancelSaleDto cancelDto);
        Task<SaleResponseDto> CreateSaleAsync(CreateSaleDto saleDto);
        Task<SaleResponseDto?> GetSaleByIdAsync(int id);
        Task<List<SaleResponseFromDataDto>> GetSalesByDateAsync(SaleRequestFromDataDto saleRequest);
    }
}
