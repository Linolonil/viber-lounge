using ViberLounge.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViberLounge.Domain.Entities
{
    public class VendaItem : BaseEntity
    {
        [Required]
        public int IdVenda { get; set; }
        public virtual Venda? Venda { get; set; }

        [Required]
        public int IdProduto { get; set; }
        public virtual Produto? Produto { get; set; }
        
        [Required]
        public int Quantidade { get; set; }
        [Required]
        public double Subtotal { get; set; }
        public bool Cancelado { get; set; } = false;
        public virtual VendaCancelada? Cancelamento { get; set; }
    }
}
