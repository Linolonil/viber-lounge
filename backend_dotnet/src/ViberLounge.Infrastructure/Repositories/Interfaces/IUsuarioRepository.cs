using ViberLounge.Domain.Entities;

namespace ViberLounge.Infrastructure.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> IsEmailExists(string email);
        Task<bool> CheckPasswordAsync(string senhaHash, string senha);
        Task AddUserAsync(Usuario Usuario);
        Task UpdateAsync(int id, Usuario input);
        Task<bool> UserExistsAsync(int id);
    }
}
