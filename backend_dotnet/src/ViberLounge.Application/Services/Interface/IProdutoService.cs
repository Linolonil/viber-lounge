using ViberLounge.Domain.Entities;
using ViberLounge.Application.DTOs.Product;

namespace ViberLounge.Application.Services.Interfaces
{
    public interface IProdutoService
    {
        Task<ProductDto> GetByIdAsync(int id);
        Task<List<ProductDto>> GetAllAsync();
        Task<Produto?> CreateProductAsync(ProductDto product);
        // Task<ProductDto> UpdateAsync(AtualizarProdutoCommand command);
        Task DeleteAsync(int id);
    }
}
