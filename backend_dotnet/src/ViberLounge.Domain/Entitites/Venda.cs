using ViberLounge.Entities;
using System.ComponentModel.DataAnnotations;

namespace ViberLounge.Domain.Entities
{
    public class Venda : BaseEntity
    {
        public Venda() => Itens = new HashSet<VendaItem>();
        public string? NomeCliente { get; set; }
        [Required]
        public int IdUsuario { get; set; }
        public virtual Usuario? Usuario { get; set; }
        [Required]
        public virtual ICollection<VendaItem> Itens { get; set; }
        [Required]
        public double PrecoTotal { get; set; }
        [Required]
        public string? Status { get; set; } = "ATIVA";
        [Required]
        public string? FormaPagamento { get; set; }
        public virtual VendaCancelada? VendaCancelada { get; set; }
    }
}