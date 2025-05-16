using ViberLounge.Application.DTOs.Sale;

namespace ViberLounge.Application.Services.Interfaces
{
    public interface ISaleService
    {
        Task<List<SaleResponseFromDataDto>> GetSalesByDateAsync(SaleRequestFromDataDto saleRequest);
        Task<SaleResponseDto> CreateAllSaleAsync(CreateSaleDto saleDto);
        Task<SaleResponseDto?> GetSaleByIdAsync(int id);
        Task<SaleResponseDto?> CancelSaleAsync(CancelSaleDto cancelDto);
    }
}
