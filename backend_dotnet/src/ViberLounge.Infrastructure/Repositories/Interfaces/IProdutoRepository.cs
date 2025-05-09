using ViberLounge.Domain.Entities;

namespace ViberLounge.Infrastructure.Repositories.Interfaces
{
    public interface IProdutoRepository
    {
        Task<Produto?> IsProductExists(string descricao);
        Task<Produto?> CreateProductAsync(Produto produto);
        Task<IEnumerable<Produto>> GetAllProductAsync();
        Task<Produto?> GetProductByIdAsync(int id);
        Task<List<Produto>> GetProductsByDescriptionAsync(string descricao);
        Task<Produto> UpdateProductAsync(Produto produto);
        Task<Produto> DeleteProductAsync(Produto produto);
        Task<Produto> RestoreProductAsync(int id);
    }
}
