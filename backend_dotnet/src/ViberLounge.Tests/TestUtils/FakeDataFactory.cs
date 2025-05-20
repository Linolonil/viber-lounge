using Bogus;
using Microsoft.AspNetCore.Http;
using ViberLounge.Domain.Entities;
using ViberLounge.Application.DTOs.Sale;
using ViberLounge.Application.DTOs.Product;

namespace ViberLounge.Tests.TestUtils;

public static class FakeDataFactory
{
    // Mocks de Produtos
    public static IEnumerable<Produto> GetFakeProducts(int quantidade = 1, int status = 1)
    {
        var product = new Faker<Produto>("pt_BR")
            .RuleFor(p => p.Id, f => f.IndexFaker + 1)
            .RuleFor(p => p.Descricao, f => f.Commerce.ProductName())
            .RuleFor(p => p.DescricaoLonga, f => f.Lorem.Paragraph(2))
            .RuleFor(p => p.Preco, f => f.Random.Double(1, 100))
            .RuleFor(p => p.ImagemUrl, f => f.Image.PicsumUrl())
            .RuleFor(p => p.Quantidade, f => status == 0 ? 0 : f.Random.Int(0, 100) + 10)
            .RuleFor(p => p.Status, f => ProdutoStatusExtensions.ToProductStatus(status))
            .RuleFor(p => p.VendaItens, f => new List<VendaItem>());

        return product.Generate(quantidade);
    }
    public static IEnumerable<CreateProductDto> GeneretadCreateProductDto(int quantidade = 1)
    {
        var createProductDto = new Faker<CreateProductDto>("pt_BR")
            .RuleFor(p => p.Descricao, f => f.Commerce.ProductName())
            .RuleFor(p => p.DescricaoLonga, f => f.Lorem.Paragraph(2))
            .RuleFor(p => p.Preco, f => f.Random.Double(1, 100))
            .RuleFor(p => p.ImagemFile, f => GetRandomImageFile())
            .RuleFor(p => p.Quantidade, f => f.Random.Int(0, 100));

        return createProductDto.Generate(quantidade);
    }

    public static IFormFile GetRandomImageFile()
    {
        var faker = new Faker();
        var fileName = faker.System.FileName("webp");
        var contentType = "image/webp";
        var imageBytes = faker.Random.Bytes(66108);

        var stream = new MemoryStream(imageBytes);
        stream.Position = 0;
        return new FormFile(stream, 0, imageBytes.Length, "ImagemFile", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
    public static SearchProductDto GenerateProductWithIdOrDescription(int? id = null, string? descricao = null)
    {
        var searchTerm = new Faker<SearchProductDto>("pt_BR")
            .RuleFor(p => p.Id, f => id)
            .RuleFor(p => p.Descricao, f => descricao);

        return searchTerm.Generate(1).First();
    }
    public static UpdateProductDto GenerateUpdateProductDto(
        int id,
        string descricao,
        string descricaoLonga,
        double preco,
        IFormFile imagemFile,
        int quantidade
    )
    {
        return new UpdateProductDto
        {
            Id = id,
            Descricao = descricao,
            DescricaoLonga = descricaoLonga,
            Preco = preco,
            ImagemFile = imagemFile,
            Quantidade = quantidade
        };
    }
    public static SaleRequestFromDataDto GenerateSaleRequestData()
    {
        var searchTerm = new Faker<SaleRequestFromDataDto>("pt_BR")
            .RuleFor(d => d.InitialDateTime, d => d.Date.Past(30, DateTime.Now))
            .RuleFor(d => d.FinalDateTime, d => d.Date.Past(30, DateTime.Now));

        return searchTerm.Generate(1).First();
    }

    // Mocks de Vendas
    public static IEnumerable<Venda> GetFakeSales(
        int quantidade = 1,
        bool isCanceled = false)
    {
        var sale = new Faker<Venda>("pt_BR")
            .RuleFor(p => p.Id, f => f.IndexFaker + 1)
            .RuleFor(p => p.NomeCliente, f => f.Name.FullName())
            .RuleFor(p => p.IdUsuario, f => GetFakeUsers().First().Id)
            .RuleFor(p => p.PrecoTotal, f => f.Random.Double(1, 100))
            .RuleFor(p => p.Cancelado, f => isCanceled)
            .RuleFor(p => p.FormaPagamento, f => f.PickRandom("PIX", "DEBITO", "CREDITO", "DINHEIRO"))
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(30, DateTime.Now))
            .RuleFor(p => p.UpdatedAt, f => f.Date.Past(30, DateTime.Now));

        return sale.Generate(quantidade);
    }
    public static IEnumerable<SaleResponseFromDataDto> GenerateSaleResponse(
        int quantitySaleResponse = 1,
        int quantitySaleItem = 1,
        bool isCanceled = false)
    {
        var sale = new Faker<SaleResponseFromDataDto>("pt_BR")
            .RuleFor(p => p.IdSale, f => f.IndexFaker + 1)
            .RuleFor(p => p.CustomerName, f => f.Name.FullName())
            .RuleFor(p => p.UserId, f => f.IndexFaker + 1)
            .RuleFor(p => p.EmployeName, f => f.Name.FullName())
            .RuleFor(p => p.TotalSalePrice, f => f.Random.Double(1, 100))
            .RuleFor(p => p.PaymentType, f => f.PickRandom("PIX", "DEBITO", "CREDITO", "DINHEIRO"))
            .RuleFor(p => p.isCanceled, f => isCanceled)
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(30, DateTime.Now))
            .RuleFor(p => p.UpdatedAt, f => f.Date.Past(30, DateTime.Now))
            .RuleFor(p => p.Items, f => GetFakeSaleItems(quantitySaleItem));


        return sale.Generate(quantitySaleResponse);
    }
    public static IEnumerable<SaleItemResponseFromData> GetFakeSaleItems(
        int quantidade = 1,
        bool isCanceled = false)
    {
        var saleItem = new Faker<SaleItemResponseFromData>("pt_BR")
            .RuleFor(p => p.IdSaleItem, f => f.IndexFaker + 1)
            .RuleFor(p => p.ProductId, f => f.IndexFaker + 1)
            .RuleFor(p => p.Quantity, f => f.Random.Int(1, 10))
            .RuleFor(p => p.isCanceled, f => isCanceled)
            .RuleFor(p => p.TotalItemPrice, f => f.Random.Double(1, 100))
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(30, DateTime.Now))
            .RuleFor(p => p.UpdatedAt, f => f.Date.Past(30, DateTime.Now));

        return saleItem.Generate(quantidade);
    }
    // Usuario
    public static IEnumerable<Usuario> GetFakeUsers(int quantidade = 1)
    {
        var user = new Faker<Usuario>("pt_BR")
            .RuleFor(p => p.Id, f => f.IndexFaker + 1)
            .RuleFor(p => p.Nome, f => f.Name.FullName())
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.Senha, f => f.Internet.Password())
            .RuleFor(p => p.Role, f => f.PickRandom("USER", "ADMIN"))
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(30, DateTime.Now))
            .RuleFor(p => p.UpdatedAt, f => f.Date.Past(30, DateTime.Now));

        return user.Generate(quantidade);
    }
}   
