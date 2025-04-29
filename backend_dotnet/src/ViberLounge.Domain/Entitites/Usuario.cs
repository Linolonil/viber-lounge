namespace ViberLounge.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public UsuarioRole Role { get; set; }
    }

    public enum UsuarioRole
    {
        Admin,
        User
    }
}