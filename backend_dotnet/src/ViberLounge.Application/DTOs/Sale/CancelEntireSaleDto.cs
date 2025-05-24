using System.ComponentModel.DataAnnotations;

namespace ViberLounge.Application.DTOs.Sale;

public class CancelEntireSaleDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "UserId deve ser maior que zero.")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "O motivo do cancelamento é obrigatório.")]
    public string? Motivo { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CancellationId deve ser maior que zero.")]
    public int CancellationId { get; set; }
}
