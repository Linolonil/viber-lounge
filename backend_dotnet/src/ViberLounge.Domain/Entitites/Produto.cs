namespace ViberLounge.Domain.Entities
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public string ImagemUrl { get; set; }
        public int Quantidade { get; set; }
        public ProdutoStatus Status { get; set; }
    }

    public enum ProdutoStatus
    {
        Disponivel,
        Indisponivel
    }
}