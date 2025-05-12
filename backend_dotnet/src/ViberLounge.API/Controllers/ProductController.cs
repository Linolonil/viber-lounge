using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViberLounge.Application.DTOs.Product;
using ViberLounge.Application.Services.Interfaces;

namespace ViberLounge.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProductController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ProductDto>>> GetAll(bool includeDeleted = false)
    {
        try
        {
            var produtos = await _produtoService.GetAllProductAsync(includeDeleted);
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Create([FromBody] CreateProductDto product)
    {
        try{
            ProductDto? produto = await _produtoService.CreateProductAsync(product);
            return Created(string.Empty, produto);
        }catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update([FromBody] UpdateProductDto product)
    {
        try
        {
            var updatedProduct = await _produtoService.UpdateProductAsync(product);
            return Ok(updatedProduct);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("ID inválido.");

            bool result = await _produtoService.DeleteProductAsync(id);

            if (!result)
                return NotFound($"Produto com ID {id} não encontrado.");

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}