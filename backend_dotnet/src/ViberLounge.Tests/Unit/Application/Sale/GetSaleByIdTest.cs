using AutoMapper;
using ViberLounge.Tests.TestUtils;
using ViberLounge.Domain.Entities;
using ViberLounge.Application.Mapping;
using ViberLounge.Application.Services;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Tests.Unit.Application.Sale;

public class GetSaleByIdTest
{
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IVendaRepository> _saleRepositoryMock;
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;

    public GetSaleByIdTest()
    {
        _loggerMock = new Mock<ILoggerService>();
        _saleRepositoryMock = new Mock<IVendaRepository>();
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }
    // Obter venda por id, mas não existe venda
    [Fact]
    public async Task GetSaleById_ShouldReturnNull_WhenSaleDoesNotExist()
    {
        var saleId = 1;
        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        _saleRepositoryMock.Setup(x => x.GetSaleByIdAsync(saleId)).ReturnsAsync((Venda?)null);

        var result = await saleService.GetSaleByIdAsync(saleId);
        Assert.Null(result);
        _saleRepositoryMock.Verify(repo => repo.GetSaleByIdAsync(saleId), Times.Once);
    }
    // Obter venda por id com sucesso
    [Fact]
    public async Task GetSaleById_ShouldReturnSale_WhenSaleExists()
    {
        var saleId = 1;
        var saleResult = FakeDataFactory.GetFakeSales(quantidade: 1).First();
        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        _saleRepositoryMock.Setup(x => x.GetSaleByIdAsync(saleId)).ReturnsAsync(saleResult);

        var result = await saleService.GetSaleByIdAsync(saleId);
        Assert.NotNull(result);
        Assert.Equal(saleResult.Id, result.Id);
        _saleRepositoryMock.Verify(repo => repo.GetSaleByIdAsync(saleId), Times.Once);
    }
}