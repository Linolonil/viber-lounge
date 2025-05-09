using System.ComponentModel.DataAnnotations;

namespace ViberLounge.Application.DTOs.Product
{
    public class ProductDto
    {
        [Required(ErrorMessage = "A descrição do produto é obrigatório")]
        public string? Descricao { get; set; }
        
        [Required(ErrorMessage = "A descrição longa do produto é obrigatória")]
        public string? DescricaoLonga { get; set; }

        [Required(ErrorMessage = "O preço do produto é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        public double Preco { get; set; }

        [Required(ErrorMessage = "A imagem do produto é obrigatória")]
        public string? ImagemUrl { get; set; }

        [Required(ErrorMessage = "A quantidade do produto é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser maior ou igual a zero")]
        [Display(Name = "Quantidade em estoque")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^\d+$", ErrorMessage = "A quantidade deve ser um número inteiro")]
        public int Quantidade { get; set; }

    }
}
