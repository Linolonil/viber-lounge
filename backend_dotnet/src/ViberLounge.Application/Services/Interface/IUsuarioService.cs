using ViberLounge.API.Controllers;

namespace  ViberLounge.Application.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<UserDto> GetByIdAsync(int id);
        Task<List<UserDto>> GetAllAsync();
        // Task<UserDto> CreateAsync(CriarUsuarioCommand command);
        // Task<UserDto> UpdateAsync(AtualizarUsuarioCommand command);
        Task DeleteProductAsync(int id);
    }
}