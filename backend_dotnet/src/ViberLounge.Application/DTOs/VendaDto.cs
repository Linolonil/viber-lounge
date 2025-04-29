namespace ViberLounge.API.Controllers
{
    public class VendaDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
        public int? TerminalId { get; set; }
        public int PeriodoId { get; set; }
        public string Status { get; set; }
        public List<ItemVendaDto> Itens { get; set; }
        public decimal Total { get; set; }
        public string FormaPagamento { get; set; }
        public string Cliente { get; set; }
    }
}
