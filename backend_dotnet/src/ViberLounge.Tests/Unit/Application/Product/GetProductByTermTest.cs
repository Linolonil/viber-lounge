using AutoMapper;
using ViberLounge.Tests.TestUtils;
using ViberLounge.Application.Mapping;
using ViberLounge.Application.Services;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Services;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Tests.Unit.Application.ProductServiceTests
{
    public class GetProductByTermTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock;

        public GetProductByTermTest()
        {
            _produtoRepositoryMock = new Mock<IProdutoRepository>();
            _loggerMock = new Mock<ILoggerService>();
            _fileServiceMock = new Mock<IFileService>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }
        // Obter erro pois input é inválido
        [Fact]
        public async Task GetProductByTermAsync_ThrowExceptionInput()
        {
            var searchProduct = FakeDataFactory.GenerateProductWithIdOrDescription();

            var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => productService.GetProductsByTermAsync(searchProduct));

            Assert.Equal("Informe Id ou Descrição para buscar.", exception.Message);
        }
        // Obter produto por ID
        [Fact]
        public async Task GetProductByTermAsync_InputIdProduct()
        {
            int idProduct = 1;
            var searchProduct = FakeDataFactory.GenerateProductWithIdOrDescription(id: 1);
            var productList = FakeDataFactory.GetFakeProducts(2).ToList();

            var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

            _produtoRepositoryMock.Setup(repo => repo.GetProductByIdAsync(idProduct)).ReturnsAsync(productList.First());

            var result = await productService.GetProductsByTermAsync(searchProduct);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        // Obter produto por ID
        [Fact]
        public async Task GetProductByTermAsync_InputDescriptionProduct()
        {
            string produtor = "Produto Teste";
            int generatProduct = 2;
            var searchProduct = FakeDataFactory.GenerateProductWithIdOrDescription(descricao: produtor);
            var productList = FakeDataFactory.GetFakeProducts(generatProduct).ToList();

            var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

            _produtoRepositoryMock.Setup(repo => repo.GetProductsByDescriptionAsync(produtor)).ReturnsAsync(productList);

            var result = await productService.GetProductsByTermAsync(searchProduct);

            Assert.NotNull(result);
            Assert.Equal(generatProduct, result.Count());
        }
    }
}