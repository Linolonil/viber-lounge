using AutoMapper;
using ViberLounge.Domain.Entities;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Application.DTOs.Product;
using ViberLounge.Application.Services.Interfaces;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        private readonly IProdutoRepository _produtoRepository;

        public ProductService(IProdutoRepository produtoRepository, ILoggerService logger, IMapper mapper)
        {
            _mapper = mapper;
            _logger = logger;
            _produtoRepository = produtoRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByTermAsync(SearchProductDto term)
        {
            try
            {
                bool hasId  = term.Id.HasValue;
                bool hasDesc = !string.IsNullOrWhiteSpace(term.Descricao);

                if (!hasId && !hasDesc)
                    throw new Exception("Informe Id ou Descrição para buscar.");

                if (hasId)
                {
                    _logger.LogInformation("Buscando produto por ID: {id}", term.Id!);
                    var entity = await _produtoRepository.GetProductByIdAsync(term.Id!.Value);
                    if (entity == null)
                        return Enumerable.Empty<ProductDto>();

                    return [_mapper.Map<ProductDto>(entity)];
                }
                else
                {
                    _logger.LogInformation("Buscando produtos por descrição: {descricao}", term.Descricao!);
                    var list = await _produtoRepository.GetProductsByDescriptionAsync(term.Descricao!);
                    if (list == null || !list.Any())
                        return Enumerable.Empty<ProductDto>();

                    return list.Select(e => _mapper.Map<ProductDto>(e));
                }
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ProductDto>> GetAllProductAsync(bool includeDeleted = false)
        {
            _logger.LogInformation("Buscando todos os produtos");
            try{
                var produtos = await _produtoRepository.GetAllProductAsync(includeDeleted);
                if (produtos == null || !produtos.Any())
                    throw new Exception("Não há produtos cadastrados");

                return _mapper.Map<List<ProductDto>>(produtos);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProductDto?> CreateProductAsync(CreateProductDto product)
        {
            _logger.LogInformation("Criando produto com descrição: {descricao}", product.Descricao!);
            try{
                var produtoExist = await _produtoRepository.IsProductExists(product.Descricao!);
                
                if(produtoExist != null)
                    throw new Exception("Produto já existe");

                Produto produto = new (){
                    Descricao = product.Descricao,
                    DescricaoLonga = product.DescricaoLonga,
                    Preco = Convert.ToDouble(product.Preco),
                    ImagemUrl = product.ImagemUrl,
                    Quantidade = product.Quantidade,
                    Status = ProdutoStatusExtensions.ToProductStatus(product.Quantidade)
                };
                
                Produto? produtoCriado = await _produtoRepository.CreateProductAsync(produto);
                return _mapper.Map<ProductDto>(produtoCriado);
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<Produto> UpdateProductAsync(UpdateProductDto product)
        {
            _logger.LogInformation("Atualizando produto com ID: {id}", product.Id);
            try{
                var productExist = await _produtoRepository.GetProductByIdAsync(product.Id);
                if (productExist == null)
                    throw new Exception($"Produto com ID {product.Id} não encontrado.");
                    
                productExist.Descricao = product.Descricao;
                productExist.DescricaoLonga = product.DescricaoLonga;
                productExist.Preco = Convert.ToDouble(product.Preco);
                productExist.ImagemUrl = product.ImagemUrl;
                productExist.Quantidade = product.Quantidade;
                productExist.Status = ProdutoStatusExtensions.ToProductStatus(product.Quantidade);
                
                var updatedProduct = await _produtoRepository.UpdateProductAsync(productExist);
                return updatedProduct;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> DeleteProductAsync(int id)
        {
            _logger.LogInformation("Removendo produto com ID: {id}", id);
            try{
                var productExist = await _produtoRepository.GetProductByIdAsync(id);
                if (productExist == null)
                    return false;

                await _produtoRepository.DeleteProductAsync(productExist);
                return true;

            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
