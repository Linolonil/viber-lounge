using System.ComponentModel.DataAnnotations;

namespace ViberLounge.Application.DTOs.Product
{
    public class SearchProductDto
    {

        [Range(1, 999, ErrorMessage = "O ID deve estar entre 1 e 999.")]
        public int? Id { get; set; }
        
        [StringLength(100, ErrorMessage = "A descrição deve ter no máximo 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "A descrição só pode conter letras, números e espaços.")]
        public string? Descricao { get; set; }
    }
}