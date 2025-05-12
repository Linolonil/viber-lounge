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
            _logger.LogInformation("Buscando venda {SaleId}", id);
            
            return await _context.Vendas
                .Include(v => v.Itens)
                .Include(v => v.VendaCancelada)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<bool> CancelSaleAsync(Venda sale, VendaCancelada cancelamento)
        {
            _logger.LogInformation("Iniciando cancelamento da venda {SaleId}", sale.Id);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Atualiza a venda
                _context.Vendas.Update(sale);

                // Adiciona o registro de cancelamento
                await _context.VendasCanceladas.AddAsync(cancelamento);

                // Atualiza os itens da venda
                foreach (var item in sale.Itens.Where(i => i.Cancelado))
                {
                    _context.ItensVendas.Update(item);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Venda {SaleId} cancelada com sucesso", sale.Id);
                return true;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao cancelar venda {SaleId}", sale.Id);
                return false;
            }
        }

        public async Task<bool> CreateSaleWithItemsAsync(Venda sale, List<VendaItem> items)
        {
            _logger.LogInformation("Iniciando criação de venda {SaleId} com {ItemCount} itens", sale.Id, items.Count);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Vendas.AddAsync(sale);
                await _context.SaveChangesAsync();

                _logger.LogDebug("Venda {SaleId} criada com sucesso", sale.Id);

                foreach (var item in items)
                {
                    item.IdVenda = sale.Id;
                }

                await _context.ItensVendas.AddRangeAsync(items);
                await _context.SaveChangesAsync();

                _logger.LogDebug("Itens adicionados à venda {SaleId}", sale.Id);

                await transaction.CommitAsync();
                _logger.LogInformation("Venda {SaleId} e seus itens foram salvos com sucesso", sale.Id);
                return true;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao salvar venda {SaleId} e seus itens", sale.Id);
                return false;
            }
        }

        public async Task<bool> CreateSaleWithItemsAndUpdateProductsAsync(Venda sale, List<VendaItem> items, List<Produto> products)
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

                _logger.LogInformation("Venda {SaleId}, itens e produtos foram salvos com sucesso", sale.Id);
                return true;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao salvar venda {SaleId}, itens e atualizar produtos", sale.Id);
                return false;
            }
        }
    }
}
