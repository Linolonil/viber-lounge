using MediatR;
using ViberLounge.API.Controllers;
using ViberLounge.Application.Services.Interfaces;

namespace ViberLounge.Application.Services
{
    public class VendaService : IVendaService
    {
        private readonly IMediator _mediator;

        public VendaService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<VendaDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<VendaDto>> GetByPeriodoAsync(int periodoId)
        {
            throw new NotImplementedException();
        }

        public Task<List<VendaDto>> GetByUsuarioAsync(int usuarioId, DateTime? dataInicio, DateTime? dataFim)
        {
            throw new NotImplementedException();
        }

        // public async Task<VendaDto> GetByIdAsync(int id)
        // {
        //     return await _mediator.Send(new GetVendaByIdQuery { Id = id });
        // }

        // public async Task<List<VendaDto>> GetByPeriodoAsync(int periodoId)
        // {
        //     return await _mediator.Send(new GetVendasByPeriodoQuery { PeriodoId = periodoId });
        // }

        // public async Task<List<VendaDto>> GetByUsuarioAsync(int usuarioId, DateTime? dataInicio, DateTime? dataFim)
        // {
        //     return await _mediator.Send(new GetVendasByUsuarioQuery 
        //     { 
        //         UsuarioId = usuarioId,
        //         DataInicio = dataInicio,
        //         DataFim = dataFim
        //     });
        // }

        // public async Task<VendaDto> CreateAsync(CriarVendaCommand command)
        // {
        //     return await _mediator.Send(command);
        // }

        // public async Task<VendaDto> CancelAsync(CancelarVendaCommand command)
        // {
        //     return await _mediator.Send(command);
        // }
    }
}
