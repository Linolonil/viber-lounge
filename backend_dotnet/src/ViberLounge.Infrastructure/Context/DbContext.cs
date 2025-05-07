using ViberLounge.Entities;
using ViberLounge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ViberLounge.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<VendaItem> ItensVendas { get; set; }
        public DbSet<VendaCancelada> VendasCanceladas { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public override int SaveChanges()
        {
            AddTimestamps(); 
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }
        public void BaseEntityTimeStamp(){
            AddTimestamps();
        }
        private void AddTimestamps()
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                ((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    ((BaseEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}