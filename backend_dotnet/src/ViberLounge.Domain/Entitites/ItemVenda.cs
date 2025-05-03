namespace ViberLounge.Domain.Entities
{
    public class Venda
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UsuarioId { get; set; }
        public string? UsuarioNome { get; set; }
        public int? TerminalId { get; set; }
        public int PeriodoId { get; set; }
        public VendaStatus Status { get; set; }
        public int? CanceladaPorId { get; set; }
        public string? CanceladaPorNome { get; set; }
        public DateTime? CanceladaEm { get; set; }
        public string? MotivoCancelamento { get; set; }
        public List<ItemVenda>? Itens { get; set; }
        public decimal Total { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public string? Cliente { get; set; }
    }

    public class ItemVenda
    {
        public int Id { get; set; }
        public int VendaId { get; set; }
        public int ProdutoId { get; set; }
        public string? ProdutoNome { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
        public decimal Subtotal { get; set; }
        public int EstoqueAntes { get; set; }
        public int EstoqueDepois { get; set; }
    }

    public enum VendaStatus
    {
        Ativa,
        Cancelada
    }

    public enum FormaPagamento
    {
        Pix,
        Credito,
        Debito,
        Dinheiro
    }
}