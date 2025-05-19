using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Application.DTOs.Product;
using ViberLounge.Application.Services.Interfaces;

namespace ViberLounge.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class ProductController : ControllerBase
{
    private readonly ILoggerService _logger;
    private readonly IProductService _produtoService;

    public ProductController(ILoggerService logger, IProductService produtoService)
    {
        _logger = logger;
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
        _logger.LogInformation("Recebendo requisição para obter todos os produtos com includeDeleted: {includeDeleted}", includeDeleted);
        try
        {
            var produtos = await _produtoService.GetAllProductAsync(includeDeleted);
            _logger.LogInformation("Produtos obtidos com sucesso");
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos");
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
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchTerm([FromQuery] SearchProductDto searchTerm)
    {
        _logger.LogInformation("Recebendo requisição para buscar produtos com o termo: {searchTerm}", searchTerm.Descricao ?? searchTerm.Id.ToString()!);
        try
        {
            var produto = await _produtoService.GetProductsByTermAsync(searchTerm);
            if (!produto.Any()){
                _logger.LogInformation("Nenhum produto encontrado com o termo: {searchTerm}", searchTerm.Descricao ?? searchTerm.Id.ToString()!);
                return NoContent();
            }
            return Ok(produto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos com o termo: {searchTerm}", searchTerm.Descricao ?? searchTerm.Id.ToString()!);
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
    [Consumes("multipart/form-data")] // Indica que aceita form-data para upload de arquivos
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Create([FromForm] CreateProductDto product) // Use FromForm em vez de FromBody
    {
        _logger.LogInformation("Recebendo requisição para criar um novo produto: {descricao}", product.Descricao!);
        try{
            var produto = await _produtoService.CreateProductAsync(product);
            _logger.LogInformation("Produto criado com sucesso: {descricao}", product.Descricao!);
            return Created(string.Empty, produto);
        }catch(Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto: {product}", product.Descricao!);
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
    [Consumes("multipart/form-data")] // Para suportar upload de arquivo
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Update([FromForm] UpdateProductDto product) // Use FromForm
    {
        _logger.LogInformation("Recebendo requisição para atualizar o produto com Id: {id}", product.Id!);
        try
        {
            var updatedProduct = await _produtoService.UpdateProductAsync(product);
            _logger.LogInformation("Produto atualizado com sucesso: {id}", product.Id!);
            return Ok(updatedProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto com ID: {id}", product.Id!);
            if (ex.Message.Contains("Não encontrado"))
                return NotFound(new { message = ex.Message });
            return BadRequest(new { message = ex.Message });
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
        _logger.LogInformation("Recebendo requisição para remover o produto com ID: {id}", id);
        try
        {
            if (id <= 0){
                _logger.LogWarning("ID inválido: {id}", id);
                return BadRequest(new { message = "ID inválido." });
            }

            bool result = await _produtoService.DeleteProductAsync(id);

            if (!result){
                _logger.LogWarning("Produto com ID {id} não encontrado.", id);
                return NotFound(new { message = $"Produto com ID {id} não encontrado." });
            }

            _logger.LogInformation("Produto com ID {id} removido com sucesso.", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover produto com ID: {id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }
}