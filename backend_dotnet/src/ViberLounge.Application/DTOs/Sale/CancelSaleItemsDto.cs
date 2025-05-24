using System.ComponentModel.DataAnnotations;

namespace ViberLounge.Application.DTOs.Sale;

public class CancelSaleItemsDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "UserId deve ser maior que zero.")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "O motivo do cancelamento é obrigatório.")]
    public string? Motivo { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "É necessário ao menos um item para cancelar.")]
    public List<int> ItemIds { get; set; } = new();
}