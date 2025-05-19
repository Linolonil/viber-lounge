using AutoMapper;
using ViberLounge.Domain.Entities;
using ViberLounge.Tests.TestUtils;
using ViberLounge.Application.Mapping;
using ViberLounge.Application.Services;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Services;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Tests.Unit.Application.ProductServiceTests
{
    public class GetAllProductTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock;

        public GetAllProductTest()
        {
            _produtoRepositoryMock = new Mock<IProdutoRepository>();
            _loggerMock = new Mock<ILoggerService>();
            _fileServiceMock = new Mock<IFileService>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }

        // Obter todo os produtos 
        [Fact]
        public async Task GetAllProductAsync_ShouldReturnListOfProducts()
        {
            List<Produto> produtList = FakeDataFactory.GetFakeProducts(2).ToList();
            bool includeDeleted = false;
            var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

            _produtoRepositoryMock.Setup(repo => repo.GetAllProductAsync(includeDeleted)).ReturnsAsync(produtList);

            var result = await productService.GetAllProductAsync(includeDeleted);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        // Obter todo os produtos + deletados
        [Fact]
        public async Task GetAllProductIncludeDeletedAsync_ShouldReturnListOfProducts()
        {
            List<Produto> produtList = FakeDataFactory.GetFakeProducts(2).ToList();
            produtList.AddRange(FakeDataFactory.GetFakeProducts(2, 0));
            bool includeDeleted = true;
            var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

            _produtoRepositoryMock.Setup(repo => repo.GetAllProductAsync(includeDeleted)).ReturnsAsync(produtList);

            var result = await productService.GetAllProductAsync(includeDeleted);

            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
        }

        // Obter todo os produtos, mas não existe produto
        [Fact]
        public async Task GetAllProduct_ButNotExistProduct()
        {
            bool includeDeleted = true;
            var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

            _produtoRepositoryMock.Setup(repo => repo.GetAllProductAsync(includeDeleted)).ReturnsAsync(new List<Produto>());

            var exception = await Assert.ThrowsAsync<Exception>(() => productService.GetAllProductAsync(includeDeleted));

           Assert.Equal("Não há produtos cadastrados", exception.Message);
        }
    }
}