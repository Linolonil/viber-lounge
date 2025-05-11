using AutoMapper;
using ViberLounge.Domain.Entities;
using ViberLounge.Application.DTOs.Product;
using ViberLounge.Application.Services.Interfaces;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IMapper _mapper;
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(IProdutoRepository produtoRepository, IMapper mapper)
        {
            _mapper = mapper;
            _produtoRepository = produtoRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByTermAsync(SearchProductDto term)
        {
            if (term == null)
                throw new ArgumentNullException(nameof(term));

            bool hasId  = term.Id.HasValue;
            bool hasDesc = !string.IsNullOrWhiteSpace(term.Descricao);

            if (!hasId && !hasDesc)
                throw new ArgumentException("Informe Id ou Descrição para buscar.");

            if (hasId)
            {
                var entity = await _produtoRepository.GetProductByIdAsync(term.Id!.Value);
                if (entity == null)
                    return Enumerable.Empty<ProductDto>();

                return [_mapper.Map<ProductDto>(entity)];
            }
            else
            {
                var list = await _produtoRepository.GetProductsByDescriptionAsync(term.Descricao!);
                return list.Select(e => _mapper.Map<ProductDto>(e));
            }
        }

        public async Task<List<ProductDto>> GetAllProductAsync(bool includeDeleted = false)
        {
            var produtos = await _produtoRepository.GetAllProductAsync(includeDeleted);
            if (produtos == null || !produtos.Any())
            {
                throw new Exception("Não há produtos cadastrados");
            }
            return _mapper.Map<List<ProductDto>>(produtos);
        }

        public async Task<ProductDto?> CreateProductAsync(CreateProductDto product)
        {
            var produtoExist = await _produtoRepository.IsProductExists(product.Descricao!);
            if(produtoExist != null)
            {
                throw new Exception("Produto já existe");
            }

            Produto produto = new (){
                Descricao = product.Descricao,
                DescricaoLonga = product.DescricaoLonga,
                Preco = Convert.ToDouble(product.Preco),
                ImagemUrl = product.ImagemUrl,
                Quantidade = product.Quantidade,
                Status = ProdutoStatusExtensions.ToProdutoStatus(product.Quantidade)
            };
            
            Produto? produtoCriado = await _produtoRepository.CreateProductAsync(produto);
            return _mapper.Map<ProductDto>(produtoCriado);
        }
        
        public async Task<Produto> UpdateProductAsync(UpdateProductDto product)
        {
            var productExist = await _produtoRepository.GetProductByIdAsync(product.Id);
            if (productExist == null)
                throw new Exception($"Produto com ID {product.Id} não encontrado.");
                
            productExist.Descricao = product.Descricao;
            productExist.DescricaoLonga = product.DescricaoLonga;
            productExist.Preco = Convert.ToDouble(product.Preco);
            productExist.ImagemUrl = product.ImagemUrl;
            productExist.Quantidade = product.Quantidade;
            productExist.Status = ProdutoStatusExtensions.ToProdutoStatus(product.Quantidade);
            
            var updatedProduct = await _produtoRepository.UpdateProductAsync(productExist);
            return updatedProduct;
        }
        public async Task<bool> DeleteProductAsync(int id)
        {
            var productExist = await _produtoRepository.GetProductByIdAsync(id);
            if (productExist == null)
            {
                return false;
            }
            await _produtoRepository.DeleteProductAsync(productExist);
            return true;
        }
    }
}
