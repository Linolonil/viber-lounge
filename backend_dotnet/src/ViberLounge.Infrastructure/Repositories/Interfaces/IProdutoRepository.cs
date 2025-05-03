using ViberLounge.Domain.Entities;

namespace ViberLounge.Infrastructure.Repositories.Interfaces
{
    public interface IProdutoRepository
    {
        Task<Produto> GetByIdAsync(int id);
        Task<IEnumerable<Produto>> GetAllAsync();
        Task AddAsync(Produto produto);
        Task UpdateAsync(Produto produto);
        Task DeleteAsync(int id);
    }
}
