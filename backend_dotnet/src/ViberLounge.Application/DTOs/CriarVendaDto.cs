namespace ViberLounge.Application.DTOs
{
    public class CriarVendaDto
    {
        public int UsuarioId { get; set; }
        public int? TerminalId { get; set; }
        public int PeriodoId { get; set; }
        public string FormaPagamento { get; set; }
        public string Cliente { get; set; }
        public List<ItemVendaDto> Itens { get; set; }
    }
}
