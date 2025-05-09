using ViberLounge.Domain.Entities;

namespace ViberLounge.Infrastructure.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> IsEmailExists(string email);
        Task AddUserAsync(Usuario Usuario);
        Task<Usuario> GetByIdAsync(int id);
        Task<bool> CheckPasswordAsync(string senhaHash, string senha);
        Task UpdateAsync(Usuario Usuario);
    }
}
