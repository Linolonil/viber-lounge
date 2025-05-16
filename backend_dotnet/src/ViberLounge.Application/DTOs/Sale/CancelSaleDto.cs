using System.ComponentModel.DataAnnotations;

namespace ViberLounge.Application.DTOs.Sale
{
    public class CancelSaleDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId deve ser maior que zero.")]
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "O motivo do cancelamento é obrigatório")]
        public string? Motivo { get; set; }

        [Required(ErrorMessage = "O tipo de cancelamento é obrigatório")]
        [RegularExpression(@"^(ITEM|VENDA)$", ErrorMessage = "Tipo de cancelamento deve ser ITEM ou VENDA")]
        public string? TipoCancelamento { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId deve ser maior que zero.")]
        public int CancellationId { get; set; }
    }
} 