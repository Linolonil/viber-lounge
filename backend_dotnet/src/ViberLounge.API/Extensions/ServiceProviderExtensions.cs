using ViberLounge.Infrastructure.Data; // Ensure this namespace matches where ViberLoungeDbContext is defined

namespace ViberLounge.API.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            // Corrigido: obter o IServiceScopeFactory para criar o escopo
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ViberLoungeDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }
    }
}