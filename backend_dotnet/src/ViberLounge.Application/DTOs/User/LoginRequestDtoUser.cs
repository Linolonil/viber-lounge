using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "A senha é obrigatória")]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
    public string? Senha { get; set; }
}