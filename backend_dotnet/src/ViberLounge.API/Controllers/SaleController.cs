using Microsoft.AspNetCore.Mvc;
using ViberLounge.Application.DTOs.Sale;
using Microsoft.AspNetCore.Authorization;
using ViberLounge.Application.Services.Interfaces;
using ViberLounge.Infrastructure.Logging;

namespace ViberLounge.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class SaleController : ControllerBase
{
    private readonly ISaleService _vendaService;
    private readonly ILoggerService _logger;

    public SaleController(ISaleService vendaService, ILoggerService logger)
    {
        _vendaService = vendaService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todas as vendas
    /// </summary>
    /// <returns>Lista de vendas</returns>
    /// <response code="200">Retorna a lista de vendas</response>
    /// <response code="204">Se não houver vendas</response>
    /// <response code="401">Se o usuário não estiver autenticado</response>
    /// <response code="500">Se ocorrer um erro interno</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(IEnumerable<SaleResponseFromDataDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSaleFromData([FromQuery] SaleRequestFromDataDto saleFromData)
    {
        _logger.LogInformation("Recebendo requisição para obter todas as vendas");
        
        try
        {
            var result = await _vendaService.GetSalesByDateAsync(saleFromData);
            if (result == null || !result.Any())
            {
                _logger.LogWarning("Nenhuma venda encontrada entre {StartDate} e {EndDate}", saleFromData.InitialDateTime, saleFromData.FinalDateTime);
                return NoContent();
            }
            _logger.LogInformation("Retornou {venda} venda(s) entre {StartDate} e {EndDate}",result.Count,  saleFromData.InitialDateTime, saleFromData.FinalDateTime);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter vendas");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro ao processar a requisição", error = ex.Message });
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
        _logger.LogInformation("Recebendo requisição para buscar venda {SaleId}", id);
        try
        {
            if (id <= 0){
                _logger.LogWarning("ID inválido fornecido: {SaleId}", id);
                return BadRequest(new { message = "ID inválido" });
            }
            
            var sale = await _vendaService.GetSaleByIdAsync(id);
            
            if (sale == null){
                _logger.LogWarning("Venda não encontrada para o ID: {SaleId}", id);
                return NotFound(new { message = "Venda não encontrada" });
            }
            _logger.LogInformation("Venda {SaleId} encontrada com sucesso", id);
            return Ok(sale);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar venda {SaleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro ao buscar venda", error = ex.Message });
        }
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
    [HttpPost("create")]
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
    /// Cancela uma venda ou item específico
    /// </summary>
    /// <param name="id">ID da venda</param>
    /// <param name="cancelDto">Dados do cancelamento</param>
    /// <returns>Venda atualizada</returns>
    /// <response code="200">Retorna a venda atualizada</response>
    /// <response code="400">Se os dados do cancelamento forem inválidos</response>
    /// <response code="404">Se a venda não for encontrada</response>
    /// <response code="401">Se o usuário não estiver autenticado</response>
    [HttpPut("cancel")]
    [ProducesResponseType(typeof(SaleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CancelSale([FromBody] CancelSaleDto cancelDto)
    {
        _logger.LogInformation("Recebendo requisição para cancelar venda {SaleId}", cancelDto.CancellationId);
        try
        {
            var result = await _vendaService.CancelSaleAsync(cancelDto);
            if (result == null)
                return NotFound(new { message = "Venda não encontrada" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar venda {SaleId}", cancelDto.CancellationId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro ao cancelar venda", error = ex.Message });
        }
    }
}