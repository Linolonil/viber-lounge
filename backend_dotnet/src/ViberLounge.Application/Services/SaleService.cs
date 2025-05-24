using AutoMapper;
using ViberLounge.Domain.Entities;
using ViberLounge.Application.DTOs.Sale;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Application.Services.Interfaces;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Application.Services
{
    public class SaleService : ISaleService
    {
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        private readonly IVendaRepository _saleRepository;
        private readonly IUsuarioRepository _userRepository;
        private readonly IProdutoRepository _productRepository;

        public SaleService(
            IVendaRepository saleRepository,
            IUsuarioRepository userRepository,
            IProdutoRepository productRepository,
            ILoggerService logger,
            IMapper mapper)
        {
            _saleRepository = saleRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _logger = logger;
            _mapper = mapper;
        }
        
        private async Task<Produto> RestoreStockProductAsync(VendaItem item)
        {
            var produto = await _productRepository.GetProductByIdAsync(item.IdProduto);
            if (produto != null)
            {
                produto.Quantidade += item.Quantidade;
                if (produto.Status == "INDISPONIVEL" && produto.Quantidade > 0)
                {
                    produto.Status = "DISPONIVEL";
                }
            }
            return produto!;
        }

        private async Task<Venda?> ProcessSaleCancellation(CancelEntireSaleDto cancel)
        {
            var sale = await _saleRepository.GetSaleByIdAsync(cancel.CancellationId);
            if (sale == null)
            {
                _logger.LogWarning("Venda não encontrada para cancelamento com ID {SaleId}", cancel.CancellationId);
                throw new Exception("Venda não encontrada");
            }
            if (sale.Cancelado == true)
            {
                _logger.LogWarning("Venda {SaleId} já está cancelada", sale.Id);
                throw new Exception("Venda já cancelada");
            }

            // Marcar venda e itens como cancelados
            sale.Cancelado = true;
            foreach (var item in sale.Itens)
            {
                item.Cancelado = true;
            }

            // Cria registro de cancelamento da venda
            var cancelSale = new VendaCancelada
            {
                IdVenda = cancel.CancellationId,
                Motivo = cancel.Motivo,
                TipoCancelamento = "VENDA",
                IdUsuario = cancel.UserId
            };

            // Cria registros de cancelamento para cada item
            var cancelSaleItems = sale.Itens.Select(item => new VendaCancelada
            {
                IdVenda = cancel.CancellationId,
                IdVendaItem = item.Id,
                Motivo = cancel.Motivo,
                TipoCancelamento = "ITEM",
                IdUsuario = cancel.UserId
            });

            var allCancelRegisters = new List<VendaCancelada> { cancelSale };
            allCancelRegisters.AddRange(cancelSaleItems);

            // Atualiza estoque dos produtos
            List<Produto> products = new();
            foreach (var item in sale.Itens)
            {
                var product = await RestoreStockProductAsync(item);
                products.Add(product);
            }

            Venda? vendaCanceld = await _saleRepository.CancelSaleAsync(sale, cancelSale, products);
            return vendaCanceld;
        }
        private async Task<(VendaItem,Produto, VendaCancelada)> ProcessSaleItemSancellation(int idItem, CancelSaleItemsDto cancel)
        {
            _logger.LogInformation("Iniciando validação de itens para ser cancelamento do Item {SaleId}", idItem);
            var item = await _saleRepository.GetSaleItemByIdAsync(idItem);
            if (item == null)
            {
                _logger.LogWarning("Item não encontrado para cancelamento com ID {ItemId}", idItem);
                throw new Exception("Item não encontrado");
            }
            if (item.Cancelado == true)
            {
                _logger.LogWarning("Item {ItemId} já está cancelado", item.Id);
                throw new Exception("Item já cancelado"); // VALIDAR SE PRECISAR RETORNAR ERRO
            }

            VendaCancelada cancelSale = new()
            {
                IdVendaItem = idItem,
                Motivo = cancel.Motivo,
                TipoCancelamento = "ITEM",
                IdUsuario = cancel.UserId
            };
            Produto product = await RestoreStockProductAsync(item);
            return (item, product, cancelSale);
        }
        
        public async Task<Venda?> CancelSaleAsync(int saleId, CancelEntireSaleDto sale)
        {
            _logger.LogInformation("Iniciando cancelamento da venda {SaleId}", saleId);

            bool userExists = await _userRepository.UserExistsAsync(sale.UserId);
            if (!userExists)
            {
                _logger.LogWarning("Tentativa de cancelar a venda, mas o usuário é inexistente {UserId}", sale.UserId);
                throw new Exception("Usuário não encontrado");
            }
            _logger.LogInformation("Iniciando cancelamento da Venda {SaleId}", saleId);
            Venda? saleResult = await ProcessSaleCancellation(sale);
            return saleResult;
        }

        public async Task<List<VendaItem>> CancelSaleItemAsync(int saleId, CancelSaleItemsDto cancelSaleItems)
        {
            _logger.LogInformation("Iniciando cancelamento do item {SaleId}", saleId);

            bool userExists = await _userRepository.UserExistsAsync(cancelSaleItems.UserId);
            if (!userExists)
            {
                _logger.LogWarning("Tentativa de cancelar a venda, mas o usuário é inexistente {UserId}", cancelSaleItems.UserId);
                throw new Exception("Usuário não encontrado");
            }
            List<Produto> products = new();
            List<VendaItem> items = new();
            List<VendaCancelada> cancelSale = new();
            foreach (var item in cancelSaleItems.ItemIds)
            {
                var (itemCancel, productCacenl, itemCancelData) = await ProcessSaleItemSancellation(item, cancelSaleItems);
                items.Add(itemCancel);
                products.Add(productCacenl);
                cancelSale.Add(itemCancelData);
            }

            List<VendaItem> vendaCanceld = await _saleRepository.CancelSaleItemAsync(items, cancelSale, products);
            return vendaCanceld;
        }

        public async Task<SaleResponseDto?> GetSaleByIdAsync(int id)
        {
            _logger.LogInformation("Iniciando busca pela venda {SaleId}", id);
            try
            {

                var venda = await _saleRepository.GetSaleByIdAsync(id);
                if (venda == null)
                    return null;
                return _mapper.Map<SaleResponseDto>(venda);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<SaleResponseFromDataDto>> GetSalesByDateAsync(SaleRequestFromDataDto saleRequest)
        {
            try
            {
                _logger.LogInformation("Iniciando busca de vendas entre {StartDate} e {EndDate}", saleRequest.InitialDateTime, saleRequest.FinalDateTime);

                var sales = await _saleRepository.GetSalesByDateAsync(saleRequest.InitialDateTime, saleRequest.FinalDateTime);
                if (sales == null || !sales.Any())
                {
                    _logger.LogWarning("Nenhuma venda encontrada entre {StartDate} e {EndDate}", saleRequest.InitialDateTime, saleRequest.FinalDateTime);
                    return new List<SaleResponseFromDataDto>();
                }

                return _mapper.Map<List<SaleResponseFromDataDto>>(sales);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        // Cria uma nova venda
        public async Task<SaleResponseDto> CreateSaleAsync(CreateSaleDto saleDto)
        {
            _logger.LogInformation("Iniciando criação de venda para usuário {UserId}", saleDto.UserId);

            if (saleDto.Items == null || saleDto.Items.Count == 0)
            {
                _logger.LogWarning("Tentativa de criar venda sem itens para usuário {UserId}", saleDto.UserId);
                throw new Exception("Nenhum item informado na venda");
            }

            bool userExists = await _userRepository.UserExistsAsync(saleDto.UserId);
            if (!userExists)
            {
                _logger.LogWarning("Tentativa de criar venda para usuário inexistente {UserId}", saleDto.UserId);
                throw new Exception("Usuário não encontrado");
            }

            List<dynamic> groupedItems = saleDto.Items
                .GroupBy(item => item.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    Quantity = group.Sum(item => item.Quantity),
                    Subtotal = group.Sum(item => item.Subtotal)
                })
                .ToList<dynamic>();

            _logger.LogDebug("Itens agrupados: {GroupedItemsCount} produtos únicos", groupedItems.Count);

            var sale = new Venda
            {
                NomeCliente = saleDto.CustomerName ?? "NAO_INFORMADO",
                IdUsuario = saleDto.UserId,
                PrecoTotal = Convert.ToDouble(saleDto.TotalPrice!),
                FormaPagamento = saleDto.PaymentType,
            };

            _logger.LogDebug("Venda criada com total de {TotalPrice} e forma de pagamento {PaymentType}", sale.PrecoTotal, sale.FormaPagamento!);

            // Processa items e prepara produtos para ser atualiazados o estoque
            var (saleItems, productsToUpdate) = await ProcessSaleItems(groupedItems);

            // Valida o subtotal de cada item e da venda total
            ProcessTotalPriceSale(saleItems, productsToUpdate, sale.PrecoTotal);

            var vendaPersistida = await _saleRepository.CreateSaleWithItemsAndUpdateProductsAsync(sale, saleItems, productsToUpdate);

            if (vendaPersistida == null)
            {
                _logger.LogError(new Exception("Falha ao criar venda e atualizar produtos"), "Erro ao criar venda e atualizar produtos para usuário {UserId}", saleDto.UserId);
                throw new Exception("Erro ao criar venda e atualizar produtos.");
            }

            _logger.LogInformation("Venda criada com sucesso para usuário {UserId} com {ItemCount} itens",
                saleDto.UserId, saleItems.Count);

            return _mapper.Map<SaleResponseDto>(vendaPersistida);
        }

        // Função usada para processar os itens da venda e atualizar os produtos(SaleCreateAsync)
        private async Task<(List<VendaItem>, List<Produto>)> ProcessSaleItems(List<dynamic> groupedItems)
        {
            var saleItems = new List<VendaItem>();
            var productsToUpdate = new List<Produto>();

            foreach (var groupedItem in groupedItems)
            {
                var product = await _productRepository.GetProductByIdAndAvailableStatus(groupedItem.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("Produto {ProductId} não encontrado. Quantidade solicitada: {Quantity}",
                        groupedItem.ProductId, groupedItem.Quantity);
                    throw new Exception("Produto não encontrado.");
                }

                if (product.Quantidade < groupedItem.Quantity)
                {
                    _logger.LogWarning("Produto {ProductId} com quantidade insuficiente. Quantidade solicitada: {Quantity}, Disponível: {Available}",
                        groupedItem.ProductId, groupedItem.Quantity, product.Quantidade);
                    throw new Exception("Quantidade insuficiente em estoque.");
                }

                product.Quantidade -= groupedItem.Quantity;
                if (product.Quantidade == 0)
                {
                    product.Status = "INDISPONIVEL";
                    _logger.LogInformation("Produto {ProductId} ficou indisponível após a venda", product.Id);
                }

                productsToUpdate.Add(product);

                saleItems.Add(new VendaItem
                {
                    IdProduto = groupedItem.ProductId,
                    Quantidade = groupedItem.Quantity,
                    Subtotal = Convert.ToDouble(groupedItem.Subtotal)
                });

                _logger.LogDebug("Item agrupado adicionado à venda: Produto {ProductId}, Quantidade {Quantity}, Subtotal {Subtotal}",
                    groupedItem.ProductId, groupedItem.Quantity, groupedItem.Subtotal);
            }

            if (saleItems.Count == 0)
            {
                throw new Exception("Nenhum item válido para salvar.");
            }

            return (saleItems, productsToUpdate);
        }

        // Função usado para validar o subtotal de cada item e o total da venda(CreateSaleAsync)
        private void ProcessTotalPriceSale(List<VendaItem> saleItems, List<Produto> products, double totalPriceSale)
        {
            //Valida o subtotal de cada item
            foreach (var item in saleItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.IdProduto);
                if (product == null)
                {
                    _logger.LogWarning("Produto com ID {ProductId} não encontrado para validação", item.IdProduto);
                    throw new Exception("Produto não encontrado para validação do subtotal");
                }

                var expectedSubtotal = product.Preco * item.Quantidade;
                if (Math.Abs(item.Subtotal - expectedSubtotal) > 0.01)
                {
                    _logger.LogWarning("Subtotal do item {ItemId} ({Subtotal}) não corresponde ao preço calculado ({ExpectedSubtotal})",
                        item.IdProduto, item.Subtotal, expectedSubtotal);
                    throw new Exception("O subtotal do item não corresponde ao preço unitário x quantidade.");
                }
            }

            // Remove duplicate calculation
            var totalPriceSaleItems = saleItems.Sum(item => item.Subtotal);

            // valida o total da venda
            if (Math.Abs(totalPriceSaleItems - totalPriceSale) > 0.01)
            {
                _logger.LogWarning("Total informado ({TotalInformado}) não corresponde ao total calculado ({TotalCalculado})",
                    totalPriceSale, totalPriceSaleItems);
                throw new Exception("O total da venda não corresponde à soma dos itens.");
            }
        }
    }
}
