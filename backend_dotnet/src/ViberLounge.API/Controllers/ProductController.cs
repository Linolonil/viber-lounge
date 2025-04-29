namespace ViberLounge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProdutoDto>>> GetAll()
    {
        var produtos = await _produtoService.GetAllAsync();
        return Ok(produtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProdutoDto>> GetById(int id)
    {
        var produto = await _produtoService.GetByIdAsync(id);
        if (produto == null) return NotFound();
        return Ok(produto);
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoDto>> Create(CriarProdutoCommand command)
    {
        var produto = await _produtoService.CreateAsync(command);
        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
    }
}