using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViberLounge.Application.DTOs.Product;
using ViberLounge.Application.Services.Interfaces;

namespace ViberLounge.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class ProductController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProductController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    /// <summary>
    /// Obtém todos os produtos
    /// </summary>
    /// <param name="includeDeleted">Incluir produtos deletados</param>
    /// <returns>Lista de produtos</returns>
    /// <response code="200">Retorna a lista de produtos</response>
    /// <response code="400">Se ocorrer um erro ao buscar os produtos</response>
    /// <response code="401">Se o usuário não estiver autenticado</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ProductDto>>> GetAll(bool includeDeleted = false)
    {
        try
        {
            var produtos = await _produtoService.GetAllProductAsync(includeDeleted);
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Busca produtos por termo
    /// </summary>
    /// <param name="searchTerm">Termo de busca</param>
    /// <returns>Lista de produtos encontrados</returns>
    /// <response code="200">Retorna a lista de produtos encontrados</response>
    /// <response code="204">Se nenhum produto for encontrado</response>
    /// <response code="400">Se ocorrer um erro na busca</response>
    /// <response code="401">Se o usuário não estiver autenticado</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    /// <param name="product">Dados do produto</param>
    /// <returns>Produto criado</returns>
    /// <response code="201">Retorna o produto criado</response>
    /// <response code="400">Se os dados do produto forem inválidos</response>
    /// <response code="401">Se o usuário não estiver autenticado</response>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Create([FromBody] CreateProductDto product)
    {
        try{
            ProductDto? produto = await _produtoService.CreateProductAsync(product);
            return Created(string.Empty, produto);
        }catch(Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    /// <param name="product">Dados atualizados do produto</param>
    /// <returns>Produto atualizado</returns>
    /// <response code="200">Retorna o produto atualizado</response>
    /// <response code="404">Se o produto não for encontrado</response>
    /// <response code="401">Se o usuário não estiver autenticado</response>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Update([FromBody] UpdateProductDto product)
    {
        try
        {
            var updatedProduct = await _produtoService.UpdateProductAsync(product);
            return Ok(updatedProduct);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remove um produto
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Produto removido com sucesso</response>
    /// <response code="400">Se o ID for inválido</response>
    /// <response code="404">Se o produto não for encontrado</response>
    /// <response code="401">Se o usuário não estiver autenticado</response>
    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { message = "ID inválido." });

            bool result = await _produtoService.DeleteProductAsync(id);

            if (!result)
                return NotFound(new { message = $"Produto com ID {id} não encontrado." });

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}