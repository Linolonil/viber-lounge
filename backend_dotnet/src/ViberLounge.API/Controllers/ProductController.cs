using Microsoft.AspNetCore.Mvc;
using ViberLounge.Application.DTOs.Product;
using ViberLounge.Application.Services.Interfaces;
using ViberLounge.Domain.Entities;

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
        var produtos = await _produtoService.GetAllAsync();
        return Ok(produtos);
    }

    [HttpGet("{id}")]
    [ValidateModel]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var produto = await _produtoService.GetByIdAsync(id);
        if (produto == null) return NotFound();
        return Ok(produto);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<ActionResult<ProductDto>> Create(ProductDto product)
    {
        Produto? produto = await _produtoService.CreateProductAsync(product);
        if (produto == null)
        {
            return BadRequest("Produto não criado");
        }
        return Ok("Produto criado com sucesso");
    }
}