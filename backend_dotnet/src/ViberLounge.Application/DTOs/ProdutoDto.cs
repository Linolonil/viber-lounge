namespace ViberLounge.API.Controllers
{
    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public string ImagemUrl { get; set; }
        public int Quantidade { get; set; }
        public string Status { get; set; }
    }
}
