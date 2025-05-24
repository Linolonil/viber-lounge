using ViberLounge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ViberLounge.Infrastructure.Context;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Infrastructure.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoggerService _logger;

        public VendaRepository(ApplicationDbContext context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Venda?> GetSaleByIdAsync(int id)
        {
            try{
                return await _context.Vendas
                    .AsNoTracking()
                    .Include(v => v.Itens)
                    // .Include(v => v.VendaCancelada)
                    .FirstOrDefaultAsync(v => v.Id == id);
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar venda com ID {SaleId}", id);
                throw;
            }
        }
        public async Task<VendaItem?> GetSaleItemByIdAsync(int id)
        {
            try{
                return await _context.ItensVendas
                    .AsNoTracking()
                    .Include(i => i.Produto)
                    .Include(i => i.Cancelamento)
                    .FirstOrDefaultAsync(i => i.Id == id);
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar venda com ID {SaleId}", id);
                throw;
            }
        }

        // public async Task<bool> CreateSaleWithItemsAsync(Venda sale, List<VendaItem> items)
        // {
        //     _logger.LogInformation("Iniciando criação de venda {SaleId} com {ItemCount} itens", sale.Id, items.Count);

        //     using var transaction = await _context.Database.BeginTransactionAsync();

        //     try
        //     {
        //         await _context.Vendas.AddAsync(sale);
        //         await _context.SaveChangesAsync();

        //         _logger.LogDebug("Venda {SaleId} criada com sucesso", sale.Id);

        //         foreach (var item in items)
        //         {
        //             item.IdVenda = sale.Id;
        //         }

        //         await _context.ItensVendas.AddRangeAsync(items);
        //         await _context.SaveChangesAsync();

        //         _logger.LogDebug("Itens adicionados à venda {SaleId}", sale.Id);

        //         await transaction.CommitAsync();
        //         _logger.LogInformation("Venda {SaleId} e seus itens foram salvos com sucesso", sale.Id);
        //         return true;
        //     }
        //     catch (DbUpdateException ex)
        //     {
        //         await transaction.RollbackAsync();
        //         _logger.LogError(ex, "Erro ao salvar venda {SaleId} e seus itens", sale.Id);
        //         return false;
        //     }
        // }

        public async Task<Venda?> CreateSaleWithItemsAndUpdateProductsAsync(Venda sale, List<VendaItem> items, List<Produto> products)
        {
            _logger.LogInformation("Iniciando criação de venda {SaleId} com {ItemCount} itens e atualização de {ProductCount} produtos", 
                sale.Id, items.Count, products.Count);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Adiciona a venda
                await _context.Vendas.AddAsync(sale);
                await _context.SaveChangesAsync();

                _logger.LogDebug("Venda {SaleId} criada com sucesso", sale.Id);

                // Adiciona os itens da venda
                foreach (var item in items)
                {
                    item.IdVenda = sale.Id;
                }
                await _context.ItensVendas.AddRangeAsync(items);

                _logger.LogDebug("Itens adicionados à venda {SaleId}", sale.Id);

                // Atualiza os produtos
                foreach (var product in products)
                {
                    _logger.LogDebug("Atualizando produto {ProductId} - Nova quantidade: {Quantity}", 
                        product.Id, product.Quantidade);
                    _context.Produtos.Update(product);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                // Carrega a venda com os itens atualizados
                var vendaCompleta = await _context.Vendas
                    .Include(v => v.Itens)
                    .FirstOrDefaultAsync(v => v.Id == sale.Id);
                    
                _logger.LogInformation("Venda {SaleId}, itens e produtos foram salvos com sucesso", sale.Id);
                return vendaCompleta;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao salvar venda {SaleId}, itens e atualizar produtos", sale.Id);
                return null;
            }
        }

        public Task<List<Venda>> GetSalesByDateAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var endDatePlusOneMs = endDate.AddMilliseconds(1);

                _logger.LogInformation(
                    "Buscando vendas entre {StartDate} e {EndDate}", 
                    startDate.ToString("HH:mm:ss.fff"), 
                    endDatePlusOneMs.ToString("HH:mm:ss.fff"));

                return _context.Vendas
                    .AsNoTracking()
                    .Include(v => v.Itens)
                    .Include(v => v.Usuario)
                    .Include(v => v.VendaCancelada)
                    .Where(v => v.CreatedAt >= startDate && v.CreatedAt < endDatePlusOneMs) // note o uso de "<"
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar vendas entre {StartDate} e {EndDate}", startDate, endDate);
                throw;
            }
        }
        public async Task<Venda?> CancelSaleAsync(Venda sale, VendaCancelada cancel, List<Produto> products)
        {
            _logger.LogInformation("Iniciando cancelamento da venda {SaleId}", sale.Id);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Atualizando os produtos
                foreach (var product in products)
                {
                    _logger.LogDebug("Atualizando produto {ProductId} - Nova quantidade: {Quantity}", product.Id, product.Quantidade);
                    _context.Produtos.Update(product);
                }

                // Cancelando os itens da venda
                foreach (var item in sale.Itens)
                {
                    item.Cancelado = true;
                    _context.ItensVendas.Update(item);
                }

                // Cancelando a venda
                sale.Cancelado = true;
                _context.Vendas.Update(sale);

                // Registrando o cancelamento
                await _context.VendasCanceladas.AddAsync(cancel);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Venda {SaleId} cancelada com sucesso", sale.Id);

                // Retorna a venda atualizada
                return await _context.Vendas
                    .Include(v => v.Itens)
                    .Include(v => v.VendaCancelada)
                    .FirstOrDefaultAsync(v => v.Id == sale.Id);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao cancelar venda {SaleId}", sale.Id);
                return null;
            }
        }
        
        public async Task<List<VendaItem>> CancelSaleItemAsync(List<VendaItem> items, List<VendaCancelada> cancelamentos, List<Produto> products)
        {
            _logger.LogInformation("Iniciando cancelamento de {Count} itens", items.Count);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Atualizando os produtos
                foreach (var product in products)
                {
                    _context.Produtos.Update(product);
                }

                // Cancelando os itens da venda
                foreach (var item in items)
                {
                    item.Cancelado = true;
                    _context.ItensVendas.Update(item);
                }

                // Registrando os cancelamentos
                await _context.VendasCanceladas.AddRangeAsync(cancelamentos);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("{Count} itens cancelados com sucesso", items.Count);

                // Retorna os itens atualizados
                var updatedItems = await _context.ItensVendas
                    .Include(i => i.Produto)
                    .Include(i => i.Cancelamento)
                    .Where(i => items.Select(x => x.Id).Contains(i.Id))
                    .ToListAsync();

                if (updatedItems.Count != items.Count)
                {
                    _logger.LogError(new Exception(), "Nem todos os itens foram encontrados após o cancelamento.");
                    throw new InvalidOperationException("Nem todos os itens de venda foram encontrados após o cancelamento.");
                }

                return updatedItems;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao cancelar itens");
                throw;
            }
        }
    }
}
