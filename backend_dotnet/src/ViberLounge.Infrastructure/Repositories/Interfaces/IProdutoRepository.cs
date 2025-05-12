using ViberLounge.Domain.Entities;

namespace ViberLounge.Infrastructure.Repositories.Interfaces
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> GetAllProductAsync(bool includeDeleted = false);
        Task<Produto?> IsProductExists(string descricao);
        Task<Produto?> CreateProductAsync(Produto produto);
        Task<Produto?> GetProductByIdAsync(int id);
        Task<List<Produto>> GetProductsByDescriptionAsync(string descricao);
        Task<Produto> UpdateProductAsync(Produto produto);
        Task<List<Produto>> UpdateProductStockAsync(List<Produto> produtos);
        Task<Produto> DeleteProductAsync(Produto produto);
        Task<Produto> RestoreProductAsync(int id);
        Task<Produto?> GetProductByIdAndAvailableStatus(int id);
    }
}
