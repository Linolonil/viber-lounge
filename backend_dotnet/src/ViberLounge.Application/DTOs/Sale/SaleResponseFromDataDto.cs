namespace ViberLounge.Application.DTOs.Sale
{
    public class SaleResponseFromDataDto
    {
        public int IdSale { get; set; }
        public string? CustomerName { get; set; }
        public int UserId { get; set; }
        public string? EmployeName { get; set; }
        public double TotalSalePrice { get; set; }
        public string? PaymentType { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<SaleItemResponseFromData> Items { get; set; } = new();
    }

    public class SaleItemResponseFromData
    {
        public int IdSaleItem { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double TotalItemPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}