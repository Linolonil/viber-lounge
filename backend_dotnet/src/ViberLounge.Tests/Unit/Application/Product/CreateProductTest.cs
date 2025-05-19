using AutoMapper;
using Microsoft.AspNetCore.Http;
using ViberLounge.Tests.TestUtils;
using ViberLounge.Domain.Entities;
using ViberLounge.Application.Mapping;
using ViberLounge.Application.Services;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Services;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Tests.Unit.Application.ProductServiceTests;

public class CreateProductTest
{
    private readonly IMapper _mapper;
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IFileService> _fileServiceMock;

    public CreateProductTest()
    {
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _loggerMock = new Mock<ILoggerService>();
        _fileServiceMock = new Mock<IFileService>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    //Teste quando prodito já existe
    [Fact]
    public async Task CreateProductAsync_ShouldThrowException_WhenProductAlreadyExists()
    {
        // Arrange
        int quantidade = 1;
        var createProductDto = FakeDataFactory.GeneretadCreateProductDto(quantidade).First();
        var existingProduct = new Produto { Id = 1, Descricao = createProductDto.Descricao };

        _produtoRepositoryMock.Setup(x => x.IsProductExists(It.IsAny<string>()))
            .ReturnsAsync(existingProduct);

        var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => productService.CreateProductAsync(createProductDto));
        Assert.Equal("Produto já existe", exception.Message);
    }
    //Teste quando não é possível salvar a imagem
    [Fact]
    public async Task CreateProductAsync_ShouldThrowException_WhenImageUploadFails()
    {
        // Arrange
        int quantidade = 1;
        var createProductDto = FakeDataFactory.GeneretadCreateProductDto(quantidade).First();
        var mockFile = new Mock<IFormFile>().Object;
        createProductDto.ImagemFile = mockFile;

        _produtoRepositoryMock.Setup(x => x.IsProductExists(It.IsAny<string>()))
            .ReturnsAsync((Produto)null!);
        _fileServiceMock.Setup(x => x.SaveFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync(string.Empty);

        var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => productService.CreateProductAsync(createProductDto));
        Assert.Equal("Erro ao salvar a imagem do produto", exception.Message);
    }

    // Teste de criação de produto com sucesso
    [Fact]
    public async Task CreateProductAsync_ShouldReturnCreatedProduct_WhenProductIsValid()
    {
        int quantidade = 1;
        var createProductDto = FakeDataFactory.GeneretadCreateProductDto(quantidade).First();
        
        string imageUrl = "/images/test-image.jpg";
        var expectedProduct = new Produto
        {
            Id = 1,
            Descricao = createProductDto.Descricao,
            DescricaoLonga = createProductDto.DescricaoLonga,
            Preco = Convert.ToDouble(createProductDto.Preco),
            ImagemUrl = imageUrl,
            Quantidade = createProductDto.Quantidade,
            Status = ProdutoStatusExtensions.ToProductStatus(createProductDto.Quantidade)
        };

        _produtoRepositoryMock.Setup(x => x.IsProductExists(It.IsAny<string>()))
            .ReturnsAsync((Produto)null!);
        _fileServiceMock.Setup(x => x.SaveFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync(imageUrl);
        _produtoRepositoryMock.Setup(x => x.CreateProductAsync(It.IsAny<Produto>()))
            .ReturnsAsync(expectedProduct);

        var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

        var result = await productService.CreateProductAsync(createProductDto);

        Assert.NotNull(result);
        Assert.Equal(expectedProduct.Id, result.Id);
        Assert.Equal(expectedProduct.Descricao, result.Descricao);
        Assert.Equal(expectedProduct.Preco, result.Preco);
        _produtoRepositoryMock.Verify(x => x.CreateProductAsync(It.IsAny<Produto>()), Times.Once);
    }

}