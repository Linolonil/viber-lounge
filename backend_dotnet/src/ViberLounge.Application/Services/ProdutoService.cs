using MediatR;
using ViberLounge.Application.DTOs.Product;
using ViberLounge.Application.Services.Interfaces;
using ViberLounge.Domain.Entities;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        Task<ProductDto> IProdutoService.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<List<ProductDto>> IProdutoService.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Produto?> CreateProductAsync(ProductDto product)
        {
            var produtoExist = await _produtoRepository.IsProductExists(product.Descricao!);
            if(produtoExist != null)
            {
                throw new Exception("Produto já existe");
            }

            Produto produto = new (){
                Descricao = product.Descricao,
                DescricaoLonga = product.DescricaoLonga,
                Preco = product.Preco,
                ImagemUrl = product.ImagemUrl,
                Quantidade = product.Quantidade,
                Status = ProdutoStatusExtensions.ToProdutoStatus(product.Quantidade)
            };
            
            var produtoCriado = await _produtoRepository.CreateProductAsync(produto);
            return produtoCriado;
        }
    }
}
