using ViberLounge.API.Controllers;

namespace  ViberLounge.Application.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioDto> GetByIdAsync(int id);
        Task<List<UsuarioDto>> GetAllAsync();
        // Task<UsuarioDto> CreateAsync(CriarUsuarioCommand command);
        // Task<UsuarioDto> UpdateAsync(AtualizarUsuarioCommand command);
        Task DeleteProductAsync(int id);
    }
}