namespace ViberLounge.Application.Services.Interfaces
{
    public interface IVendaService
    {
        Task<VendaDto> GetByIdAsync(int id);
        Task<List<VendaDto>> GetByPeriodoAsync(int periodoId);
        Task<List<VendaDto>> GetByUsuarioAsync(int usuarioId, DateTime? dataInicio, DateTime? dataFim);
        Task<VendaDto> CreateAsync(CriarVendaCommand command);
        Task<VendaDto> CancelAsync(CancelarVendaCommand command);
    }
}
