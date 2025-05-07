using System.ComponentModel.DataAnnotations;
using ViberLounge.Entities;

namespace ViberLounge.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        [Required]
        public string? Nome { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Senha { get; set; }
        [Required]
        public string? Role { get; set; }
    }
    public static class UsuarioRoleExtensions
    {
        public static UsuarioRole ToUsuarioRole(this string role)
        {
            return role switch
            {
                "ADMIN" => UsuarioRole.ADMIN,
                "USER" => UsuarioRole.USER,
                _ => throw new ArgumentException("Role Inválida")
            };
        }
    }
    public enum UsuarioRole { ADMIN, USER }
}