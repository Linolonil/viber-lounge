namespace ViberLounge.Application.Services.Interfaces
{
    public interface IProdutoService
    {
        Task<ProdutoDto> GetByIdAsync(int id);
        Task<List<ProdutoDto>> GetAllAsync();
        Task<ProdutoDto> CreateAsync(CriarProdutoCommand command);
        Task<ProdutoDto> UpdateAsync(AtualizarProdutoCommand command);
        Task DeleteAsync(int id);
    }
}
