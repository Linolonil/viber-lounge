using AutoMapper;
using ViberLounge.Tests.TestUtils;
using ViberLounge.Domain.Entities;
using ViberLounge.Application.Mapping;
using ViberLounge.Application.Services;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Repositories.Interfaces;
using ViberLounge.Application.DTOs.Sale;

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

    // Criando venda com dois produtos, mas ocorreu uma exeção pois o segundo produto não existe
    [Fact]
    public async Task CreatingSaleWithTwoProduct_ButProductSecundNotExiste()
    {
        var userExists = true;
        var saleResquest = FakeDataFactory.GenerateRequestCreateSaleDto(quantitySale: 1, quantitySaleitems: 2);
        var firtProduct = saleResquest.First().Items![0].ProductId;
        var secundProduct = saleResquest.First().Items![1].ProductId;
        var productData = FakeDataFactory.GetFakeProducts().First();

        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        _usuarioRepositoryMock.Setup(u => u.UserExistsAsync(saleResquest.First().UserId)).ReturnsAsync(userExists);
        _produtoRepositoryMock.Setup(p => p.GetProductByIdAndAvailableStatus(firtProduct)).ReturnsAsync(productData);
        _produtoRepositoryMock.Setup(p => p.GetProductByIdAndAvailableStatus(secundProduct)).ReturnsAsync((Produto)null!);
        var execption = await Assert.ThrowsAsync<Exception>(() => saleService.CreateSaleAsync(saleResquest.First()));

        Assert.Equal("Produto não encontrado.", execption.Message);
    }

    // Criando venda com dois produtos, mas ocorreu uma exeção pois o segundo produto tem estoque insuficiente
    [Fact]
    public async Task CreatingSaleWithTwoProduct_ButProductSecundThisQuantityInsufficient()
    {
        var userExists = true;
        var saleResquest = FakeDataFactory.GenerateRequestCreateSaleDto(quantitySale: 1, quantitySaleitems: 2);
        saleResquest.First().Items![1].Quantity = 2;
        var firtProductId = saleResquest.First().Items![0].ProductId;
        var secundProductId = saleResquest.First().Items![1].ProductId;

        var productData = FakeDataFactory.GetFakeProducts(2);
        var firtProductData = productData.ToList()[0];
        var secundProductData = productData.ToList()[1];
        secundProductData.Quantidade = 1;

        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        _usuarioRepositoryMock.Setup(u => u.UserExistsAsync(saleResquest.First().UserId)).ReturnsAsync(userExists);
        _produtoRepositoryMock.Setup(p => p.GetProductByIdAndAvailableStatus(firtProductId)).ReturnsAsync(firtProductData);
        _produtoRepositoryMock.Setup(p => p.GetProductByIdAndAvailableStatus(secundProductId)).ReturnsAsync(secundProductData);
        var execption = await Assert.ThrowsAsync<Exception>(() => saleService.CreateSaleAsync(saleResquest.First()));

        Assert.Equal("Quantidade insuficiente em estoque.", execption.Message);
    }

    [Theory]
    [InlineData(100.00, 40.00, 50.00)]
    [InlineData(100.00, 80.00, 100.00)]
    [InlineData(50.00, 10.00, 100.00)]
    public async Task CreatingSaleWithTwoProduct_ButThereExceptionBecauseHasDivergentSubtotal
    (double totalPriceSale, double totalPriceSaleItemI, double totalPriceSaleItemII)
    {
        bool userExists = true;

        CreateSaleDto saleResquest = FakeDataFactory.GenerateSaleDtoValid(totalPrice: totalPriceSale);
        Saleitems saleItemI = FakeDataFactory.GenerateSaleItemsValid(quantity: 1, subtotal: totalPriceSaleItemI);
        Saleitems saleItemII = FakeDataFactory.GenerateSaleItemsValid(quantity: 1, subtotal: totalPriceSaleItemII);
        saleItemII.ProductId = 2;
        saleResquest.Items = new List<Saleitems> { saleItemI, saleItemII };

        Produto produtoDataI = FakeDataFactory.GerenateProductValid(price: totalPriceSaleItemI);
        Produto produtoDataII = FakeDataFactory.GerenateProductValid(price: totalPriceSaleItemII);
        produtoDataII.Id = 2;

        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        _usuarioRepositoryMock.Setup(u => u.UserExistsAsync(saleResquest.UserId)).ReturnsAsync(userExists);
        _produtoRepositoryMock.Setup(p => p.GetProductByIdAndAvailableStatus(saleItemI.ProductId)).ReturnsAsync(produtoDataI);
        _produtoRepositoryMock.Setup(p => p.GetProductByIdAndAvailableStatus(saleItemII.ProductId)).ReturnsAsync(produtoDataII);
        var execption = await Assert.ThrowsAsync<Exception>(() => saleService.CreateSaleAsync(saleResquest));

        Assert.Equal("O total da venda não corresponde à soma dos itens.", execption.Message);
    }
}

// Test Plan:
// Test when sale has no items - ok
// Test when user doesn't exist - ok
// Test when product is not available - ok
// Test when product quantity is insufficient - ok
// Test when calculated total doesn't match provided total - ok
// Test when calculated subtotal doesn't match provided subtotal - ok

// Test successful sale creation
// Test when repository operation fails