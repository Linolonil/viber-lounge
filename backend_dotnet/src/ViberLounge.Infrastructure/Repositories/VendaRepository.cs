namespace ViberLounge.Infrastructure.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly ViberLoungeDbContext _context;

        public VendaRepository(ViberLoungeDbContext context)
        {
            _context = context;
        }

        public async Task<Venda> GetByIdAsync(int id)
        {
            return await _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Venda>> GetAllAsync()
        {
            return await _context.Vendas
                .Include(v => v.Itens)
                .ToListAsync();
        }

        public async Task AddAsync(Venda venda)
        {
            await _context.Vendas.AddAsync(venda);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Venda venda)
        {
            _context.Vendas.Update(venda);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Venda>> GetByPeriodoAsync(int periodoId)
        {
            return await _context.Vendas
                .Include(v => v.Itens)
                .Where(v => v.PeriodoId == periodoId)
                .ToListAsync();
        }
    }
}
