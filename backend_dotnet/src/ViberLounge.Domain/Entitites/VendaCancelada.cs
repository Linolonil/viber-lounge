using ViberLounge.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViberLounge.Domain.Entities
{
    public class VendaCancelada : BaseEntity
    {
        [Required]
        public int IdVenda { get; set; }
        public virtual Venda? Venda { get; set; }

        public int? IdVendaItem { get; set; }
        public virtual VendaItem? VendaItem { get; set; }
        
        [Required]
        public int IdUsuario { get; set; }
        public virtual Usuario? Usuario { get; set; }
        
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