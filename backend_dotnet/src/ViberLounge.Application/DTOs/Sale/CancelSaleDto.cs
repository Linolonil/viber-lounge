using System.ComponentModel.DataAnnotations;

namespace ViberLounge.Application.DTOs.Sale
{
    public class CancelSaleDto
    {
        [Required(ErrorMessage = "O motivo do cancelamento é obrigatório")]
        public string? Motivo { get; set; }

        [Required(ErrorMessage = "O tipo de cancelamento é obrigatório")]
        [RegularExpression(@"^(ITEM|VENDA)$", ErrorMessage = "Tipo de cancelamento deve ser ITEM ou VENDA")]
        public string? TipoCancelamento { get; set; }

        public int? ItemId { get; set; }
    }
} 