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
    // Cancelar venda, mas não existe venda
    [Fact]
    public async Task CancelSale_ShouldReturnFalse_WhenSaleDoesNotExist()
    {
        
        var saleCancel = FakeDataFactory.GenerateCancelSaleDto(tipoCancelamento: "VENDA");
        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        _usuarioRepositoryMock.Setup(x => x.UserExistsAsync(saleCancel.UserId)).ReturnsAsync();

        var result = await Assert.ThrowsAsync<Exception>(() => saleService.CancelSaleAsync(saleCancel));
        Assert.False(result);
        _saleRepositoryMock.Verify(repo => repo.GetSaleByIdAsync(saleId), Times.Once);
    }
    // Cancelar venda com sucesso
    [Fact]
    public async Task CancelSale_ShouldReturnTrue_WhenSaleExists()
    {
        var saleId = 1;
        var saleResult = FakeDataFactory.GetFakeSales(quantidade: 1).First();
        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        _saleRepositoryMock.Setup(x => x.GetSaleByIdAsync(saleId)).ReturnsAsync(saleResult);
        _saleRepositoryMock.Setup(x => x.UpdateSaleAsync(saleResult)).ReturnsAsync(true);

        var result = await saleService.CancelSaleAsync(saleId);
        Assert.True(result);
        _saleRepositoryMock.Verify(repo => repo.GetSaleByIdAsync(saleId), Times.Once);
        _saleRepositoryMock.Verify(repo => repo.UpdateSaleAsync(saleResult), Times.Once);
    }
    // Cancelar venda, mas não foi possível atualizar
    [Fact]
    public async Task CancelSale_ShouldReturnFalse_WhenUpdateFails()
    {
        var saleId = 1;
        var saleResult = FakeDataFactory.GetFakeSales(quantidade: 1).First();
        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        _saleRepositoryMock.Setup(x => x.GetSaleByIdAsync(saleId)).ReturnsAsync(saleResult);
        _saleRepositoryMock.Setup(x => x.UpdateSaleAsync(saleResult)).ReturnsAsync(false);

        var result = await saleService.CancelSaleAsync(saleId);
        Assert.False(result);
        _saleRepositoryMock.Verify(repo => repo.GetSaleByIdAsync(saleId), Times.Once);
        _saleRepositoryMock.Verify(repo => repo.UpdateSaleAsync(saleResult), Times.Once);
    }
    // Cancelar venda, mas não foi possível atualizar
}