using System.ComponentModel.DataAnnotations;

namespace ViberLounge.Application.DTOs.Sale;
public class CreateSaleDto
{
    public string? CustomerName { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "UserId deve ser maior que zero.")]
    public int UserId { get; set; }
    [Required]
    public double TotalPrice { get; set; }
    [Required]
    [RegularExpression(@"^(PIX|DEBITO|CREDITO|DINHEIRO)$", ErrorMessage = "PaymentType aceita apenas: PIX, DEBITO, CREDITO ou DINHEIRO. Em letras maiúsculas.")]
    public string? PaymentType { get; set; }
    [Required]
    public List<Saleitems>? Items { get; set; }
}
public class Saleitems {
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ProductId deve ser maior que zero.")]
    public int ProductId { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity deve ser maior que zero.")]
    public int Quantity { get; set; }
    [Required]
    public double Subtotal { get; set; }
}