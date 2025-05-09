namespace ViberLounge.Application.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        
        public string? Descricao { get; set; }
        public string? DescricaoLonga { get; set; }
        public double Preco { get; set; }
        public string? ImagemUrl { get; set; }
        public int Quantidade { get; set; }
        public string? Status { get; set; }
    }
}