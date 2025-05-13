using ViberLounge.Domain.Entities;
using ViberLounge.Application.DTOs.Product;

namespace ViberLounge.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllProductAsync(bool includeDeleted = false);
        Task<IEnumerable<ProductDto>> GetProductsByTermAsync(SearchProductDto searchTerm);
        Task<ProductDto?> CreateProductAsync(CreateProductDto product);
        Task<Produto> UpdateProductAsync(UpdateProductDto product);
        Task<bool> DeleteProductAsync(int id);
    }
}
