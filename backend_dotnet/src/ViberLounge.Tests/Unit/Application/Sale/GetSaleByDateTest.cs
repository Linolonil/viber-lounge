using AutoMapper;
using ViberLounge.Tests.TestUtils;
using ViberLounge.Application.Mapping;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Tests.Unit.Application.Sale;

public class GetSaleByDateTest
{
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IVendaRepository> _ventaRepositoryMock;
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;

    public GetSaleByDateTest()
    {
        _loggerMock = new Mock<ILoggerService>();
        _ventaRepositoryMock = new Mock<IVendaRepository>();
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task GetSaleByDate_ShouldReturnEmptyList_WhenNoSalesExist()
    {
        var saleRequest = FakeDataFactory.GenerateSaleRequestData();
        Console.WriteLine($"Sale Request: {saleRequest.InitialDateTime} - {saleRequest.FinalDateTime}");
    }

    [Fact]
    public async Task GetSaleByDate_ShouldReturnSales_WhenSalesExistOnGivenDate()
    {

    }
}