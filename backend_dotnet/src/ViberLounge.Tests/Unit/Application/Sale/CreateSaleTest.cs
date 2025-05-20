using AutoMapper;
using ViberLounge.Tests.TestUtils;
using ViberLounge.Domain.Entities;
using ViberLounge.Application.Mapping;
using ViberLounge.Application.Services;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Tests.Unit.Application.Sale;

public class CreateSaleTest
{
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IVendaRepository> _saleRepositoryMock;
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;

    public CreateSaleTest()
    {
        _loggerMock = new Mock<ILoggerService>();
        _saleRepositoryMock = new Mock<IVendaRepository>();
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    // Criando venda, mas nenhum item(produto) foi informado
    [Fact]
    public async Task CreatingSale_ButSaleNotContainsItemsSale()
    {
        var saleResquest = FakeDataFactory.GenerateRequestCreateSaleDto(quantitySaleitems: 0);

        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        var execption = await Assert.ThrowsAsync<Exception>(() => saleService.CreateSaleAsync(saleResquest.First()));

        Assert.Equal("Nenhum item informado na venda", execption.Message);
    }

    // Criando venda, mas usuario não existe
    [Fact]
    public async Task CreatingSale_ButSaleNotContainsUserExiste()
    {
        var userExists = false;
        var saleResquest = FakeDataFactory.GenerateRequestCreateSaleDto();

        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        _usuarioRepositoryMock.Setup(u => u.UserExistsAsync(saleResquest.First().UserId)).ReturnsAsync(userExists);

        var execption = await Assert.ThrowsAsync<Exception>(() => saleService.CreateSaleAsync(saleResquest.First()));

        Assert.Equal("Usuário não encontrado", execption.Message);
    }
}

// Test Plan:
// Test when sale has no items
// Test when user doesn't exist
// Test when product is not available
// Test when product quantity is insufficient
// Test when calculated total doesn't match provided total
// Test successful sale creation
// Test when repository operation fails