using AutoMapper;
using ViberLounge.Domain.Entities;
using ViberLounge.Tests.TestUtils;
using ViberLounge.Application.Mapping;
using ViberLounge.Application.Services;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Services;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Tests.Unit.Application.ProductServiceTests;

public class DeleteProductTest
{
    private readonly IMapper _mapper;
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IFileService> _fileServiceMock;

    public DeleteProductTest()
    {
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _loggerMock = new Mock<ILoggerService>();
        _fileServiceMock = new Mock<IFileService>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    // Delete produto sem sucesso, pois produto não existe
    [Fact]
    public async Task DeleteProduct_ShouldThrowException_WhenProductNotFound()
    {
        // Arrange
        int productId = 1;
        _produtoRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync((Produto)null!);

        var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

        var exception = await Assert.ThrowsAsync<Exception>(() => productService.DeleteProductAsync(productId));
        Assert.Equal("Produto não encontrado", exception.Message);
    }

    // Deletar produto com sucesso, mas logger é ativado pois não tem uma imagem associada
    [Fact]
    public async Task DeleteProductSucess_WithLogMessageProductWithOutImage()
    {
        // Arrange
        int productId = 1;
        var product = FakeDataFactory.GetFakeProducts(1).First();
        product.ImagemUrl = null;
        product.IsDeleted = true;

        _produtoRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);
        _produtoRepositoryMock.Setup(repo => repo.DeleteProductAsync(product)).ReturnsAsync(product);

        var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

        bool result = await productService.DeleteProductAsync(productId);
        _loggerMock.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce());
        Assert.True(result);
    }

    // Deletar produto com sucesso, e chama o método DeleteFile do FileService
    [Fact]
    public async Task DeleteProduct_ShouldCallRepositoryDeleteAsync()
    {
        // Arrange
        int productId = 1;
        var product = FakeDataFactory.GetFakeProducts(1).First();
        product.IsDeleted = true;

        _produtoRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);
        _fileServiceMock.Setup(file => file.DeleteFile(product.ImagemUrl!)).Returns(true);
        _produtoRepositoryMock.Setup(repo => repo.DeleteProductAsync(product)).ReturnsAsync(product);

        var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

        bool result = await productService.DeleteProductAsync(productId);
        _produtoRepositoryMock.Verify(repo => repo.DeleteProductAsync(product), Times.Once);
        Assert.True(result);
    }
}