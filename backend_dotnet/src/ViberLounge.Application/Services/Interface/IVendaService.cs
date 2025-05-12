using ViberLounge.Application.DTOs.Sale;

namespace ViberLounge.Application.Services.Interfaces
{
    public interface IVendaService
    {
        Task<SaleResponseDto> CreateAllSaleAsync(CreateSaleDto saleDto);
        Task<SaleResponseDto?> GetSaleByIdAsync(int id);
        Task<SaleResponseDto?> CancelSaleAsync(int id, CancelSaleDto cancelDto);
    }
}
