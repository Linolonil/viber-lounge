using Microsoft.AspNetCore.Mvc;
using ViberLounge.Application.DTOs.Sale;
using Microsoft.AspNetCore.Authorization;
using ViberLounge.Application.Services.Interfaces;

namespace ViberLounge.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class SaleController : ControllerBase
{
    private readonly IVendaService _vendaService;
    private readonly ILogger<SaleController> _logger;

    public SaleController(IVendaService vendaService, ILogger<SaleController> logger)
    {
        _vendaService = vendaService;
        _logger = logger;
    }

    /// <summary>
    /// Cria uma nova venda
    /// </summary>
    /// <param name="saleDto">Dados da venda</param>
    /// <returns>Venda criada</returns>
    /// <response code="201">Retorna a venda criada</response>
    /// <response code="400">Se os dados da venda forem inválidos</response>
    /// <response code="401">Se o usuário não estiver autenticado</response>
    /// <response code="500">Se ocorrer um erro interno</response>
    [HttpPost]
    [ProducesResponseType(typeof(SaleResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleDto saleDto)
    {
        _logger.LogInformation("Recebendo requisição para criar venda");
        try
        {
            var result = await _vendaService.CreateAllSaleAsync(saleDto);
            return CreatedAtAction(nameof(GetSaleById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar venda");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro ao processar a venda", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém uma venda pelo ID
    /// </summary>
    /// <param name="id">ID da venda</param>
    /// <returns>Venda encontrada</returns>
    /// <response code="200">Retorna a venda solicitada</response>
    /// <response code="404">Se a venda não for encontrada</response>
    /// <response code="401">Se o usuário não estiver autenticado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SaleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSaleById(int id)
    {
        try
        {
            var sale = await _vendaService.GetSaleByIdAsync(id);
            if (sale == null)
                return NotFound(new { message = "Venda não encontrada" });

            return Ok(sale);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar venda {SaleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro ao buscar venda", error = ex.Message });
        }
    }

    /// <summary>
    /// Cancela uma venda ou item específico
    /// </summary>
    /// <param name="id">ID da venda</param>
    /// <param name="cancelDto">Dados do cancelamento</param>
    /// <returns>Venda atualizada</returns>
    /// <response code="200">Retorna a venda atualizada</response>
    /// <response code="400">Se os dados do cancelamento forem inválidos</response>
    /// <response code="404">Se a venda não for encontrada</response>
    /// <response code="401">Se o usuário não estiver autenticado</response>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(typeof(SaleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CancelSale(int id, [FromBody] CancelSaleDto cancelDto)
    {
        _logger.LogInformation("Recebendo requisição para cancelar venda {SaleId}", id);
        try
        {
            var result = await _vendaService.CancelSaleAsync(id, cancelDto);
            if (result == null)
                return NotFound(new { message = "Venda não encontrada" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar venda {SaleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro ao cancelar venda", error = ex.Message });
        }
    }
}