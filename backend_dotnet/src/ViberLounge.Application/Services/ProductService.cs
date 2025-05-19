using AutoMapper;
using ViberLounge.Domain.Entities;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Services;
using ViberLounge.Application.DTOs.Product;
using ViberLounge.Application.Services.Interfaces;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        private readonly IFileService _fileService;
        private readonly IProdutoRepository _produtoRepository;

        public ProductService(IMapper mapper, ILoggerService logger, IFileService fileService,  IProdutoRepository produtoRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _fileService = fileService;
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

                string imageUrl = string.Empty;
                if (product.ImagemFile != null && product.ImagemFile.Length > 0)
                {
                    imageUrl = await _fileService.SaveFileAsync(product.ImagemFile);
                }
                if (string.IsNullOrEmpty(imageUrl))
                    throw new Exception("Erro ao salvar a imagem do produto");

                Produto produto = new (){
                    Descricao = product.Descricao,
                    DescricaoLonga = product.DescricaoLonga,
                    Preco = Convert.ToDouble(product.Preco),
                    ImagemUrl = imageUrl,
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
        
        public async Task<ProductDto> UpdateProductAsync(UpdateProductDto product)
        {
            _logger.LogInformation("Atualizando produto com ID: {id}", product.Id);
            try{
                var productExist = await _produtoRepository.GetProductByIdAsync(product.Id);
                if (productExist == null)
                    throw new Exception($"Produto com ID {product.Id} não encontrado.");
                
                string imageUrl = string.Empty;
                if (product.ImagemFile != null && product.ImagemFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(productExist.ImagemUrl) && 
                        productExist.ImagemUrl.StartsWith("/images/"))
                    {
                        _fileService.DeleteFile(productExist.ImagemUrl);
                    }
                    
                    imageUrl = await _fileService.SaveFileAsync(product.ImagemFile);
                }
                    
                productExist.Descricao = product.Descricao;
                productExist.DescricaoLonga = product.DescricaoLonga;
                productExist.Preco = Convert.ToDouble(product.Preco);
                productExist.ImagemUrl = imageUrl ?? productExist.ImagemUrl;
                productExist.Quantidade = product.Quantidade;
                productExist.Status = ProdutoStatusExtensions.ToProductStatus(product.Quantidade);
                
                var updatedProduct = await _produtoRepository.UpdateProductAsync(productExist);
                
                return _mapper.Map<ProductDto>(updatedProduct);
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
                    throw new Exception("Produto não encontrado");

                var deletedProduct = await _produtoRepository.DeleteProductAsync(productExist);

                if (deletedProduct.IsDeleted)
                {
                    if (!_fileService.DeleteFile(deletedProduct.ImagemUrl ?? string.Empty))
                    {
                        _logger.LogWarning("Imagem do produto com ID {id} não encontrada ou não é uma imagem válida.", id);  
                    }else{
                        _logger.LogInformation("Imagem do produto com ID {id} deletada com sucesso.", id);
                    }
                }
                return deletedProduct.IsDeleted;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
