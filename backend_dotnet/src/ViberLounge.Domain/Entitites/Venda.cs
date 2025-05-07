using System.ComponentModel.DataAnnotations;
using ViberLounge.Entities;

namespace ViberLounge.Domain.Entities
{
    public class Venda : BaseEntity
    {
        public string? NomeCliente { get; set; }
        [Required]
        public int IdUsuario { get; set; }
        [Required]
        public List<VendaItem>? Itens { get; set; }
        [Required]
        public double PrecoTotal { get; set; }
        [Required]
        public string? Status { get; set; }
        [Required]
        public string? FormaPagamento { get; set; }
    }
    public static class VendaStatusExtensions
    {
        public static VendaStatus ToVendaStatus(this string status)
        {
            return status switch
            {
                "ATIVA" => VendaStatus.ATIVA,
                "CANCELADA" => VendaStatus.CANCELADA,
                _ => throw new ArgumentException("Status Inválido")
            };
        }
    }
    public static class FormaPagamentoExtensions
    {
        public static FormaPagamento ToFormaPagamento(this string formaPagamento)
        {
            return formaPagamento switch
            {
                "PIX" => FormaPagamento.PIX,
                "CREDITO" => FormaPagamento.CREDITO,
                "DEBITO" => FormaPagamento.DEBITO,
                "DINHEIRO" => FormaPagamento.DINHEIRO,
                _ => throw new ArgumentException("Forma de Pagamento Inválida")
            };
        }
    }
    public enum FormaPagamento{ PIX, CREDITO, DEBITO, DINHEIRO}

    public enum VendaStatus{ ATIVA, CANCELADA}
}