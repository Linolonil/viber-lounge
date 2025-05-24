using AutoMapper;
using ViberLounge.Tests.TestUtils;
using ViberLounge.Domain.Entities;
using ViberLounge.Application.Mapping;
using ViberLounge.Application.Services;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Repositories.Interfaces;
using Xunit.Sdk;

namespace ViberLounge.Tests.Unit.Application.Sale;

public class CancelSaleTest
{
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IVendaRepository> _saleRepositoryMock;
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;

    public CancelSaleTest()
    {
        _loggerMock = new Mock<ILoggerService>();
        _saleRepositoryMock = new Mock<IVendaRepository>();
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }
    // Cancelar venda, mas usuario existe venda
    [Fact]
    public async Task CancelSale_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        bool isUserExists = false;
        int saleId = 1;
        var saleCancel = FakeDataFactory.GenerateCancelSaleDto();
        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        _usuarioRepositoryMock.Setup(x => x.UserExistsAsync(saleCancel.UserId)).ReturnsAsync(isUserExists);

        var exception = await Assert.ThrowsAsync<Exception>(() => saleService.CancelSaleAsync(saleId, saleCancel));
        Assert.Equal("Usuário não encontrado", exception.Message);
    }
    // Cancelar venda, mas venda não existe
    [Fact]
    public async Task CancelSale_ShouldReturnFalse_WhenSaleDoesNotExist()
    {
        var saleId = 1;
        var saleCancel = FakeDataFactory.GenerateCancelSaleDto();
        bool isUserExists = true;
        Venda? saleResult = null;
        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        _usuarioRepositoryMock.Setup(u => u.UserExistsAsync(saleCancel.UserId)).ReturnsAsync(isUserExists);
        _saleRepositoryMock.Setup(s => s.GetSaleByIdAsync(saleId)).ReturnsAsync(saleResult);

        var exception = await Assert.ThrowsAsync<Exception>(() => saleService.CancelSaleAsync(saleId, saleCancel));

        _usuarioRepositoryMock.Verify(user => user.UserExistsAsync(saleId), Times.Once);
        _saleRepositoryMock.Verify(sale => sale.GetSaleByIdAsync(saleId), Times.Once);
        Assert.Equal("Venda não encontrada", exception.Message);
    }

    // Cancelar venda, mas venda já foi cancelada
    [Fact]
    public async Task CancelSale_ShouldReturnFalse_SaleHasAlreadyBeenCancelled()
    {
        var saleId = 1;
        var saleCancel = FakeDataFactory.GenerateCancelSaleDto();
        bool isUserExists = true;
        Venda? saleResult = FakeDataFactory.GetFakeSales(quantidade: 1, isCanceled: true).First();
        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        _usuarioRepositoryMock.Setup(u => u.UserExistsAsync(saleCancel.UserId)).ReturnsAsync(isUserExists);
        _saleRepositoryMock.Setup(s => s.GetSaleByIdAsync(saleId)).ReturnsAsync(saleResult);

        var exception = await Assert.ThrowsAsync<Exception>(() => saleService.CancelSaleAsync(saleId, saleCancel));

        _usuarioRepositoryMock.Verify(user => user.UserExistsAsync(saleId), Times.Once);
        _saleRepositoryMock.Verify(sale => sale.GetSaleByIdAsync(saleId), Times.Once);
        Assert.Equal("Venda já cancelada", exception.Message);
    }

    // Cancelar venda com sucesso
    [Fact]
    public async Task CancelSaleTwoProductSucess()
    {
        var saleId = 1;
        var saleCancel = FakeDataFactory.GenerateCancelSaleDto();
        bool isUserExists = true;

        //Criando venda com 2 produtos
        Venda? saleResult = FakeDataFactory.GenerateVendaValid();
        List<VendaItem> saleItemResult = FakeDataFactory.GenerateVendaItensValid(quantidade: 2).ToList();
        saleResult.Itens = saleItemResult;

        //Criando produtos
        List<Produto> productResult = FakeDataFactory.GetFakeProducts(quantidade: 2).ToList();

        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        _usuarioRepositoryMock.Setup(u => u.UserExistsAsync(saleCancel.UserId)).ReturnsAsync(isUserExists);
        _saleRepositoryMock.Setup(s => s.GetSaleByIdAsync(saleId)).ReturnsAsync(saleResult);
        _produtoRepositoryMock.Setup(p => p.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(productResult[0]);
        _produtoRepositoryMock.Setup(p => p.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(productResult[1]);

        var exception = await saleService.CancelSaleAsync(saleId, saleCancel);

        _usuarioRepositoryMock.Verify(user => user.UserExistsAsync(saleId), Times.Once);
        _saleRepositoryMock.Verify(sale => sale.GetSaleByIdAsync(saleId), Times.Once);
        _produtoRepositoryMock.Verify(p => p.GetProductByIdAsync(It.IsAny<int>()), Times.Exactly(2));
    }
    // // Cancelar venda, mas não foi possível atualizar
}