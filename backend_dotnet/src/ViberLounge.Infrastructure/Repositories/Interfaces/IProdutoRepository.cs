using ViberLounge.Domain.Entities;

namespace ViberLounge.Infrastructure.Repositories.Interfaces
{
    public interface IProdutoRepository
    {
        Task<Produto?> IsProductExists(string descricao);
        Task<Produto?> CreateProductAsync(Produto produto);
        Task<IEnumerable<Produto>> GetAllAsync();
        Task UpdateAsync(Produto produto);
        Task DeleteAsync(int id);
    }
}
