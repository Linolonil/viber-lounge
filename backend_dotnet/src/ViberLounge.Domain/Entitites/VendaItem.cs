using ViberLounge.Entities;
using System.ComponentModel.DataAnnotations;

namespace ViberLounge.Domain.Entities
{
    public class VendaItem : BaseEntity
    {
        [Required]
        public int IdVenda { get; set; }
        [Required]
        public int IdProduto { get; set; }
        [Required]
        public int Quantidade { get; set; }
        [Required]
        public double Subtotal { get; set; }
    }
}
