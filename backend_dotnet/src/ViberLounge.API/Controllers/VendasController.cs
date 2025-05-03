using Microsoft.AspNetCore.Mvc;
using ViberLounge.Application.Services.Interfaces;

namespace ViberLounge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendasController : ControllerBase
{
    private readonly IVendaService _vendaService;

    public VendasController(IVendaService vendaService)
    {
        _vendaService = vendaService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VendaDto>> GetById(int id)
    {
        var venda = await _vendaService.GetByIdAsync(id);
        if (venda == null) return NotFound();
        return Ok(venda);
    }

    [HttpGet("periodo/{periodoId}")]
    public async Task<ActionResult<List<VendaDto>>> GetByPeriodo(int periodoId)
    {
        var vendas = await _vendaService.GetByPeriodoAsync(periodoId);
        return Ok(vendas);
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<List<VendaDto>>> GetByUsuario(
        int usuarioId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        var vendas = await _vendaService.GetByUsuarioAsync(usuarioId, dataInicio, dataFim);
        return Ok(vendas);
    }

    // [HttpPost]
    // public async Task<ActionResult<VendaDto>> Create(CriarVendaCommand command)
    // {
    //     var venda = await _vendaService.CreateAsync(command);
    //     return CreatedAtAction(nameof(GetById), new { id = venda.Id }, venda);
    // }

    // [HttpPost("{id}/cancelar")]
    // public async Task<ActionResult<VendaDto>> Cancel(int id, [FromBody] CancelarVendaCommand command)
    // {
    //     command.Id = id;
    //     var venda = await _vendaService.CancelAsync(command);
    //     return Ok(venda);
    // }
}