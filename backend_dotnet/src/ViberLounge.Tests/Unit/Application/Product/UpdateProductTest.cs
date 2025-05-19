using AutoMapper;
using ViberLounge.Tests.TestUtils;
using ViberLounge.Domain.Entities;
using ViberLounge.Application.Mapping;
using ViberLounge.Application.Services;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Services;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Tests.Unit.Application.ProductServiceTests
{
    public class UpdateProductTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock;

        public UpdateProductTest()
        {
            _produtoRepositoryMock = new Mock<IProdutoRepository>();
            _loggerMock = new Mock<ILoggerService>();
            _fileServiceMock = new Mock<IFileService>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }
        // Atualizar produto, mas produto não existe
        [Fact]
        public async Task UpdateProductAsync_ButProductNotExist()
        {
            var product = FakeDataFactory.GetFakeProducts(1).First();
            var productToUpdate = FakeDataFactory.GenerateUpdateProductDto(
                id: product.Id,
                descricao: product.Descricao!,
                descricaoLonga: product.DescricaoLonga!,
                preco: product.Preco,
                imagemFile: FakeDataFactory.GetRandomImageFile(),
                quantidade: product.Quantidade
             );
            _produtoRepositoryMock.Setup(repo => repo.GetProductByIdAsync(product.Id)).ReturnsAsync((Produto)null!);

            var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => productService.UpdateProductAsync(productToUpdate));

            Assert.Equal("Produto não encontrado", exception.Message);
        }

        // Atualizar produto mesmo se produto existente não tiver imagem
        [Fact]
        public async Task UpdateProductAsync_SameProductImageNotExist()
        {
            string newImageUrl = "images/product/new-image.jpg";
            var product = FakeDataFactory.GetFakeProducts(1).First();
            product.ImagemUrl = null;

            var productToUpdate = FakeDataFactory.GenerateUpdateProductDto(
                id: product.Id,
                descricao: product.Descricao!,
                descricaoLonga: product.DescricaoLonga!,
                preco: product.Preco,
                imagemFile: FakeDataFactory.GetRandomImageFile(),
                quantidade: product.Quantidade
             );

            _produtoRepositoryMock.Setup(repo => repo.GetProductByIdAsync(product.Id)).ReturnsAsync(product);
            _fileServiceMock.Setup(file => file.SaveFileAsync(productToUpdate.ImagemFile!)).ReturnsAsync(newImageUrl);
            _produtoRepositoryMock.Setup(repo => repo.UpdateProductAsync(It.IsAny<Produto>())).ReturnsAsync(product);

            var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

            var result = await productService.UpdateProductAsync(productToUpdate);

            Assert.NotNull(result);
            Assert.Equal(productToUpdate.Id, result.Id);
            Assert.Equal(productToUpdate.Descricao, result.Descricao);
            Assert.Equal(productToUpdate.DescricaoLonga, result.DescricaoLonga);
            Assert.Equal(productToUpdate.Preco, result.Preco);
            Assert.Equal(productToUpdate.Quantidade, result.Quantidade);
            Assert.NotEmpty(result.ImagemUrl!);    
        }

        // Atualizar produto, mas não salvar imagem porque ocorreu erro ao  deletar imagem antiga
        [Fact]
        public async Task UpdateProductAsync_ButDoesntSavePoductBecauseErrorOccurredWhileDeletingOldImage()
        {
            var product = FakeDataFactory.GetFakeProducts(1).First();

            var productToUpdate = FakeDataFactory.GenerateUpdateProductDto(
                id: product.Id,
                descricao: product.Descricao!,
                descricaoLonga: product.DescricaoLonga!,
                preco: product.Preco,
                imagemFile: FakeDataFactory.GetRandomImageFile(),
                quantidade: product.Quantidade
             );

            _produtoRepositoryMock.Setup(repo => repo.GetProductByIdAsync(product.Id)).ReturnsAsync(product);
            _fileServiceMock.Setup(file => file.DeleteFile(product.ImagemUrl!)).Returns(false);

            var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => productService.UpdateProductAsync(productToUpdate));

            Assert.Equal("Falha ao excluir a imagem anterior do produto ID", exception.Message);
        }

        // Atualizar produto, mas não salvar imagem porque ocorreu erro ao salvar imagem atual
        [Fact]
        public async Task UpdateProductAsync_ButDoesntSavePoductBecauseErrorOccurredWhileDeletingCurrentImage()
        {
            var product = FakeDataFactory.GetFakeProducts(1).First();

            var productToUpdate = FakeDataFactory.GenerateUpdateProductDto(
                id: product.Id,
                descricao: product.Descricao!,
                descricaoLonga: product.DescricaoLonga!,
                preco: product.Preco,
                imagemFile: FakeDataFactory.GetRandomImageFile(),
                quantidade: product.Quantidade
             );

            _produtoRepositoryMock.Setup(repo => repo.GetProductByIdAsync(product.Id)).ReturnsAsync(product);
            _fileServiceMock.Setup(file => file.DeleteFile(product.ImagemUrl!)).Returns(true);
            _fileServiceMock.Setup(file => file.SaveFileAsync(productToUpdate.ImagemFile!)).ReturnsAsync(string.Empty);

            var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => productService.UpdateProductAsync(productToUpdate));

            Assert.Equal("Não foi possível salvar a imagem do produto.", exception.Message);
        }

        // Atualizar produto com imagem e deletar imagem antiga
        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateProduct()
        {
            string newImageUrl = "images/product/new-image.jpg";
            var product = FakeDataFactory.GetFakeProducts(1).First();
            var productToUpdate = FakeDataFactory.GenerateUpdateProductDto(
                id: product.Id,
                descricao: product.Descricao!,
                descricaoLonga: product.DescricaoLonga!,
                preco: product.Preco,
                imagemFile: FakeDataFactory.GetRandomImageFile(),
                quantidade: product.Quantidade
            );

            _produtoRepositoryMock.Setup(repo => repo.GetProductByIdAsync(productToUpdate.Id)).ReturnsAsync(product);
            _fileServiceMock.Setup(file => file.DeleteFile(product.ImagemUrl!)).Returns(true);
            _fileServiceMock.Setup(file => file.SaveFileAsync(productToUpdate.ImagemFile!)).ReturnsAsync(newImageUrl);
            _produtoRepositoryMock.Setup(repo => repo.UpdateProductAsync(It.IsAny<Produto>())).ReturnsAsync(product);

            var productService = new ProductService(_mapper, _loggerMock.Object, _fileServiceMock.Object, _produtoRepositoryMock.Object);

            var result = await productService.UpdateProductAsync(productToUpdate);

            Assert.NotNull(result);
            Assert.Equal(productToUpdate.Id, result.Id);
            Assert.Equal(productToUpdate.Descricao, result.Descricao);
            Assert.Equal(productToUpdate.DescricaoLonga, result.DescricaoLonga);
            Assert.Equal(productToUpdate.Preco, result.Preco);
            Assert.Equal(productToUpdate.Quantidade, result.Quantidade);
            _produtoRepositoryMock.Verify(repo => repo.UpdateProductAsync(It.IsAny<Produto>()), Times.Once);
        }
    }
}