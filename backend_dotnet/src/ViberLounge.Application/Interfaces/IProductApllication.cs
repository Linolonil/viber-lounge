using ViberLounge.Application.DTOs.Product;

namespace ViberLounge.Application.Interfaces
{
    public interface IProductApplication
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(ProductDto ProductDto);
        Task UpdateAsync(int id, ProductDto ProductDto);
        Task DeleteProductAsync(int id);
    }
}
