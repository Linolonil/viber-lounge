using ViberLounge.API.Controllers;

namespace ViberLounge.Application.Interfaces
{
    public interface IProductApplication
    {
        Task<IEnumerable<ProdutoDto>> GetAllAsync();
        Task<ProdutoDto> GetByIdAsync(int id);
        Task<ProdutoDto> CreateAsync(ProdutoDto produtoDto);
        Task UpdateAsync(int id, ProdutoDto produtoDto);
        Task DeleteAsync(int id);
    }
}
