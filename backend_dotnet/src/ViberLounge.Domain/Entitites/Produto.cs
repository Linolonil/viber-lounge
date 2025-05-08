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
        public static ProdutoStatus ToProdutoStatus(this string status)
        {
            return status switch
            {
                "DISPONIVEL" => ProdutoStatus.DISPONIVEL,
                "INDISPONIVEL" => ProdutoStatus.INDISPONIVEL,
                _ => throw new ArgumentException("Status Inválido")
            };
        }
    }
    public enum ProdutoStatus{ DISPONIVEL, INDISPONIVEL}
}