using System.ComponentModel.DataAnnotations;

namespace ViberLounge.Application.DTOs.Product
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "O ID do produto é obrigatório")]
        public int Id { get; set; }

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
        [RegularExpression(@"^\d{1,3}$", ErrorMessage = "A quantidade deve ser um número inteiro entre 0 e 999")]
        [DataType(DataType.Text)]
        public int Quantidade { get; set; }
    }
}