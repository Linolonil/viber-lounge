namespace ViberLounge.Infrastructure.Repositories.Interfaces
{
    public interface IVendaRepository
    {
        Task<Venda> GetByIdAsync(int id);
        Task<IEnumerable<Venda>> GetAllAsync();
        Task AddAsync(Venda venda);
        Task UpdateAsync(Venda venda);
        Task<IEnumerable<Venda>> GetByPeriodoAsync(int periodoId);
    }
}