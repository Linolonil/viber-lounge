using ViberLounge.Entities;
using System.ComponentModel.DataAnnotations;

namespace ViberLounge.Domain.Entities
{
    public class VendaCancelada : BaseEntity
    {
        [Required]
        public int IdVenda { get; set; }
        [Required]
        public int IdUsuario { get; set; }
        [Required]
        public string? Motivo { get; set; }
        [Required]
        public string? TipoCancelamento { get; set; }
    }
    public static class TipoCancelamentoExtensions
    {
        public static TipoCancelamento ToTipoCancelamento(this string tipoCancelamento)
        {
            return tipoCancelamento switch
            {
                "ITEM" => TipoCancelamento.ITEM,
                "VENDA" => TipoCancelamento.VENDA,
                _ => throw new ArgumentException("Tipo de Cancelamento Inválido")
            };
        }
    }
    public enum TipoCancelamento{ ITEM, VENDA}
}