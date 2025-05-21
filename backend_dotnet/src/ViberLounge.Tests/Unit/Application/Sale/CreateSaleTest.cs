using AutoMapper;
using ViberLounge.Tests.TestUtils;
using ViberLounge.Domain.Entities;
using ViberLounge.Application.Mapping;
using ViberLounge.Application.Services;
using ViberLounge.Application.DTOs.Sale;
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

    // Criando venda com dois produtos, mas ocorreu uma exeção pois o total da venda é difernte do total dos itens
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

    // Criando venda com dois produtos, mas ocorreu uma exeção pois o subtotal do item não corresponde ao preço unitário x quantidade
    [Theory]
    [InlineData(100.00, 50.00, 50.00)]
    [InlineData(180.00, 80.00, 100.00)]
    [InlineData(110.00, 10.00, 100.00)]
    public async Task CreatingSaleWithTwoProduct_ButThereAnExceptionBecauseSubtotalValueDoesNotMatchQuantityXPrice
    (double totalPriceSale, double totalPriceSaleItemI, double totalPriceSaleItemII)
    {
        bool userExists = true;

        CreateSaleDto saleResquest = FakeDataFactory.GenerateSaleDtoValid(totalPrice: totalPriceSale);
        Saleitems saleItemI = FakeDataFactory.GenerateSaleItemsValid(quantity: 3, subtotal: totalPriceSaleItemI);
        Saleitems saleItemII = FakeDataFactory.GenerateSaleItemsValid(quantity: 4, subtotal: totalPriceSaleItemII);
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

        Assert.Equal("O subtotal do item não corresponde ao preço unitário x quantidade.", execption.Message);
    }

    // Criando venda com dois produtos com sucesso, e o preço total da venda é igual ao preço unitário x quantidade
    [Theory]
    [InlineData(100.00, 50.00, 50.00)]
    [InlineData(180.00, 80.00, 100.00)]
    [InlineData(110.00, 10.00, 100.00)]
    public async Task CreatingSaleWithTwoProduct_ProcessSucessfully
    (double totalPriceSale, double totalPriceSaleItemI, double totalPriceSaleItemII)
    {
        bool userExists = true;

        CreateSaleDto saleResquest = FakeDataFactory.GenerateSaleDtoValid(totalPrice: totalPriceSale);
        Saleitems saleItemI = FakeDataFactory.GenerateSaleItemsValid(quantity: 3, subtotal: totalPriceSaleItemI);
        Saleitems saleItemII = FakeDataFactory.GenerateSaleItemsValid(quantity: 4, subtotal: totalPriceSaleItemII);
        saleItemII.ProductId = 2;

        saleResquest.Items = new List<Saleitems> { saleItemI, saleItemII };

        Produto produtoDataI = FakeDataFactory.GerenateProductValid(price: totalPriceSaleItemI);
        Produto produtoDataII = FakeDataFactory.GerenateProductValid(price: totalPriceSaleItemII);
        produtoDataII.Id = 2;

        //Ajustando o valor do subtotal do segundo item para que seja igual ao preço unitário x quantidade
        saleItemI.Subtotal = produtoDataI.Preco * saleItemI.Quantity;
        saleItemII.Subtotal = produtoDataII.Preco * saleItemII.Quantity;

        
        // Criando mock de venda
        double totalPrice = saleItemI.Subtotal + saleItemII.Subtotal;
        Venda saleData = FakeDataFactory.GenerateVendaValid(precoTotal: totalPrice);

        // Atualizando o preço total da venda
        saleResquest.TotalPrice = totalPrice;

        //Criando mock para repositorio de venda
        VendaItem vendaItemI = FakeDataFactory.GenerateVendaItemValid(
            idVenda: saleData.Id,
            idProduto: produtoDataI.Id,
            quantidade: saleItemI.Quantity,
            subtotal: saleItemI.Subtotal);
        VendaItem vendaItemII = FakeDataFactory.GenerateVendaItemValid(
            idVenda: saleData.Id,
            idProduto: produtoDataII.Id,
            quantidade: saleItemII.Quantity,
            subtotal: saleItemII.Subtotal);

        List<Produto> produtos = new List<Produto> { produtoDataI, produtoDataII };
        List<VendaItem> saleItems = new List<VendaItem> { vendaItemI, vendaItemII };
        saleData.Itens = saleItems;

        var saleService = new SaleService(_saleRepositoryMock.Object, _usuarioRepositoryMock.Object, _produtoRepositoryMock.Object, _loggerMock.Object, _mapper);

        _usuarioRepositoryMock.Setup(u => u.UserExistsAsync(saleResquest.UserId)).ReturnsAsync(userExists);
        _produtoRepositoryMock.Setup(p => p.GetProductByIdAndAvailableStatus(saleItemI.ProductId)).ReturnsAsync(produtoDataI);
        _produtoRepositoryMock.Setup(p => p.GetProductByIdAndAvailableStatus(saleItemII.ProductId)).ReturnsAsync(produtoDataII);
        _saleRepositoryMock.Setup(s => s.CreateSaleWithItemsAndUpdateProductsAsync(
            It.IsAny<Venda>(),
            It.IsAny<List<VendaItem>>(),
            It.IsAny<List<Produto>>()))
        .ReturnsAsync(saleData);

        var result = await saleService.CreateSaleAsync(saleResquest);

        Assert.NotNull(result);
        Assert.Equal(saleData.Id, result.Id);
        Assert.Equal(saleData.PrecoTotal, result.TotalPrice);
        Assert.Equal(saleData.Itens.ToList().Count(), result.Items!.Count());
        Assert.Equal(saleData.Itens.ToList()[0].Subtotal, result.Items![0].Subtotal);
        Assert.Equal(saleData.Itens.ToList()[1].Subtotal, result.Items![1].Subtotal);
        Assert.Equal(saleData.Itens.ToList()[0].Quantidade, result.Items![0].Quantity);
        Assert.Equal(saleData.Itens.ToList()[1].Quantidade, result.Items![1].Quantity);
    }
}