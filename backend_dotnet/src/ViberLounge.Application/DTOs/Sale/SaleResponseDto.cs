
namespace ViberLounge.Application.DTOs.Sale;

public class SaleResponseDto
{
    public int Id { get; set; }
    public string? CustomerName { get; set; }
    public int UserId { get; set; }
    public double TotalPrice { get; set; }
    public string? PaymentType { get; set; }
    public bool IsCanceled { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<SaleItemResponseDto> Items { get; set; } = new();
}

public class SaleItemResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public double Subtotal { get; set; }
}
