using Microsoft.AspNetCore.Mvc;
using ViberLounge.Application.DTOs.Sale;
using Microsoft.AspNetCore.Authorization;
using ViberLounge.Application.Services.Interfaces;

namespace ViberLounge.API.Controllers;

[Authorize]
[ApiController]
[Route("sale")]
public class SaleController : ControllerBase
{
    private readonly IVendaService _vendaService;

    public SaleController(IVendaService vendaService)
    {
        _vendaService = vendaService;
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]    
    [ProducesResponseType(StatusCodes.Status400BadRequest)]	
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleDto saleDto)
    {
        try
        {
            var result = await _vendaService.CreateAsync(saleDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}