using System.ComponentModel.DataAnnotations;
using ViberLounge.Entities;

namespace ViberLounge.Domain.Entities
{
    public class Produto : BaseEntity
    {
        [Required]
        public string? Descricao { get; set; }
        [Required]
        public string? DescricaoLonga { get; set; }
        [Required]
        public double Preco { get; set; }
        [Required]
        public string? ImagemUrl { get; set; }
        [Required]
        public int Quantidade { get; set; }
        [Required]
        public string? Status { get; set; }
        public virtual ICollection<VendaItem> VendaItens { get; set; } = new HashSet<VendaItem>();
    }
    public static class ProdutoStatusExtensions
    {
        public static string ToProdutoStatus(int quantidade)
        {
            return quantidade == 0 ? "INDISPONIVEL" : "DISPONIVEL";
        }
    }
}