using ViberLounge.Entities;
using System.ComponentModel.DataAnnotations;

namespace ViberLounge.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        [Required]
        public string? Nome { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Senha { get; set; }
        [Required]
        public string? Role { get; set; } = "USER";
        public virtual ICollection<Venda> Vendas { get; set; } = new HashSet<Venda>();
        public virtual ICollection<VendaCancelada> VendasCanceladas { get; set; } = new HashSet<VendaCancelada>();
    }

}