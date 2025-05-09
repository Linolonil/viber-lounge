using ViberLounge.Application.DTOs.Product;

namespace ViberLounge.Application.Services.Interfaces
{
    public interface IProdutoService
    {
        Task<List<ProductDto>> GetAllProductAsync();
        Task<IEnumerable<ProductDto>> GetProductsByTermAsync(SearchProductDto searchTerm);
        Task<ProductDto?> CreateProductAsync(CreateProductDto product);
        // Task<ProductDto> UpdateAsync(AtualizarProdutoCommand command);
        Task DeleteAsync(int id);
    }
}
