using Bogus;
using Microsoft.AspNetCore.Http;
using ViberLounge.Domain.Entities;
using ViberLounge.Application.DTOs.Product;

namespace ViberLounge.Tests.TestUtils;

public static class FakeDataFactory
{
    public static IEnumerable<Produto> GetFakeProducts(int quantidade = 1, int status = 1)
    {
        var product = new Faker<Produto>("pt_BR")
            .RuleFor(p => p.Id, f => f.IndexFaker + 1)
            .RuleFor(p => p.Descricao, f => f.Commerce.ProductName())
            .RuleFor(p => p.DescricaoLonga, f => f.Lorem.Paragraph(2))
            .RuleFor(p => p.Preco, f => f.Random.Double(1, 100))
            .RuleFor(p => p.ImagemUrl, f => f.Image.PicsumUrl())
            .RuleFor(p => p.Quantidade, f => status == 0 ? 0 : f.Random.Int(0, 100)+ 10)
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
    
    private static IFormFile GetRandomImageFile()
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
}   
