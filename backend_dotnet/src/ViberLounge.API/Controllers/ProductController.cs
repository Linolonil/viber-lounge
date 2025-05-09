using Microsoft.AspNetCore.Mvc;
using ViberLounge.Application.DTOs.Product;
using ViberLounge.Application.Services.Interfaces;

namespace ViberLounge.API.Controllers;

[Route("product")]
public class ProductController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProductController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpGet]
    [ValidateModel]
    public async Task<ActionResult<List<ProductDto>>> GetAll()
    {
        try
        {
            var produtos = await _produtoService.GetAllProductAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("search")]
    [ValidateModel]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchT([FromQuery] SearchProductDto searchTerm)
    {
        try
        {
            var produto = await _produtoService.GetProductsByTermAsync(searchTerm);
            if (!produto.Any()) 
                return NoContent();
                
            return Ok(produto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
    [HttpPost("create")]
    [ValidateModel]
    public async Task<ActionResult> Create([FromBody] CreateProductDto product)
    {
        ProductDto? produto = await _produtoService.CreateProductAsync(product);
        if (produto == null)
        {
            return BadRequest("Produto não criado");
        }
        return Created(string.Empty, produto);
    }
}