using ViberLounge.Domain.Entities;

namespace ViberLounge.Infrastructure.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> GetByIdAsync(int id);
        Task<Usuario> GetByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(Usuario user, string senha);
        Task AddAsync(Usuario Usuario);
        Task UpdateAsync(Usuario Usuario);
    }
}
