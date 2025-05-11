using ViberLounge.Application.DTOs.Sale;

namespace ViberLounge.Application.Services.Interfaces
{
    public interface IVendaService
    {
        Task<CreateSaleDto> CreateAsync(CreateSaleDto saleDto);
    }
}
