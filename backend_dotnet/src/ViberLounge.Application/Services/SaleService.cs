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
        private readonly ILoggerService _logger;
        private readonly IVendaRepository _saleRepository;
        private readonly IUsuarioRepository _userRepository;
        private readonly IProdutoRepository _productRepository;
        private readonly IMapper _mapper;

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

        public async Task<SaleResponseDto?> CancelSaleAsync(int id, CancelSaleDto cancelDto)
        {
            _logger.LogInformation("Iniciando cancelamento da venda {SaleId}", id);

            var venda = await _saleRepository.GetSaleByIdAsync(id);
            if (venda == null)
            {
                _logger.LogWarning("Venda {SaleId} não encontrada para cancelamento", id);
                return null;
            }

            if (venda.Status == "CANCELADA")
            {
                _logger.LogWarning("Venda {SaleId} já está cancelada", id);
                throw new Exception("Esta venda já está cancelada");
            }

            var cancelamento = new VendaCancelada
            {
                IdVenda = id,
                Motivo = cancelDto.Motivo,
                TipoCancelamento = cancelDto.TipoCancelamento,
                IdUsuario = venda.IdUsuario // Usuário que está cancelando
            };

            if (cancelDto.TipoCancelamento == "ITEM" && cancelDto.ItemId.HasValue)
            {
                var item = venda.Itens.FirstOrDefault(i => i.Id == cancelDto.ItemId);
                if (item == null)
                {
                    _logger.LogWarning("Item {ItemId} não encontrado na venda {SaleId}", cancelDto.ItemId, id);
                    throw new Exception("Item não encontrado na venda");
                }

                cancelamento.IdVendaItem = item.Id;
                item.Cancelado = true;

                // Restaura a quantidade do produto
                var produto = await _productRepository.GetProductByIdAsync(item.IdProduto);
                if (produto != null)
                {
                    produto.Quantidade += item.Quantidade;
                    if (produto.Status == "INDISPONIVEL" && produto.Quantidade > 0)
                    {
                        produto.Status = "DISPONIVEL";
                    }
                    await _productRepository.UpdateProductAsync(produto);
                }
            }
            else
            {
                // Cancela a venda inteira
                venda.Status = "CANCELADA";
                
                // Restaura as quantidades de todos os produtos
                foreach (var item in venda.Itens)
                {
                    item.Cancelado = true;
                    var produto = await _productRepository.GetProductByIdAsync(item.IdProduto);
                    if (produto != null)
                    {
                        produto.Quantidade += item.Quantidade;
                        if (produto.Status == "INDISPONIVEL" && produto.Quantidade > 0)
                        {
                            produto.Status = "DISPONIVEL";
                        }
                        await _productRepository.UpdateProductAsync(produto);
                    }
                }
            }

            // Salva o cancelamento e atualiza a venda
            var sucesso = await _saleRepository.CancelSaleAsync(venda, cancelamento);
            if (!sucesso)
            {
                _logger.LogError(new Exception("Falha ao Cancelar a Venda"), "Erro ao cancelar venda {SaleId}", id);
                throw new Exception("Erro ao processar o cancelamento");
            }

            _logger.LogInformation("Venda {SaleId} cancelada com sucesso", id);
            return _mapper.Map<SaleResponseDto>(venda);
        }

        public async Task<SaleResponseDto> CreateAllSaleAsync(CreateSaleDto saleDto)
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

            var groupedItems = saleDto.Items
                .GroupBy(item => item.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    Quantity = group.Sum(item => item.Quantity),
                    Subtotal = group.Sum(item => item.Subtotal)
                })
                .ToList();

            _logger.LogDebug("Itens agrupados: {GroupedItemsCount} produtos únicos", groupedItems.Count);

            var sale = new Venda
            {
                NomeCliente = saleDto.CustomerName ?? "NAO_INFORMADO",
                IdUsuario = saleDto.UserId,
                PrecoTotal = Convert.ToDouble(saleDto.TotalPrice!),
                FormaPagamento = saleDto.PaymentType,
            };

            _logger.LogDebug("Venda criada com total de {TotalPrice} e forma de pagamento {PaymentType}", sale.PrecoTotal, sale.FormaPagamento!);

            var saleItems = new List<VendaItem>();
            var productsToUpdate = new List<Produto>();
            
            foreach (var groupedItem in groupedItems)
            {
                var product = await _productRepository.GetProductByIdAndAvailableStatus(groupedItem.ProductId);
                if (product == null || product!.Quantidade < groupedItem.Quantity)
                {
                    _logger.LogWarning("Produto {ProductId} não encontrado ou quantidade insuficiente. Quantidade solicitada: {Quantity}, Disponível: {Available}", groupedItem.ProductId, groupedItem.Quantity, product?.Quantidade ?? 0);
                    throw new Exception("Nenhum item válido para salvar.");
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
                _logger.LogWarning("Nenhum item válido encontrado para a venda do usuário {UserId}", saleDto.UserId);
                throw new Exception("Nenhum item válido para salvar.");
            }

            var totalCalculado = saleItems.Sum(item => item.Subtotal);
            if (Math.Abs(totalCalculado - sale.PrecoTotal) > 0.01)
            {
                _logger.LogWarning("Total informado ({TotalInformado}) não corresponde ao total calculado ({TotalCalculado})", sale.PrecoTotal, totalCalculado);
                throw new Exception("O total da venda não corresponde à soma dos itens.");
            }

            bool sucesso = await _saleRepository.CreateSaleWithItemsAndUpdateProductsAsync(sale, saleItems, productsToUpdate);

            if (!sucesso)
            {
                _logger.LogError(new Exception("Falha ao criar venda e atualizar produtos"), "Erro ao criar venda e atualizar produtos para usuário {UserId}", saleDto.UserId);
                throw new Exception("Erro ao criar venda e atualizar produtos.");
            }

            _logger.LogInformation("Venda criada com sucesso para usuário {UserId} com {ItemCount} itens", 
                saleDto.UserId, saleItems.Count);

            return _mapper.Map<SaleResponseDto>(sale);
        }

        public async Task<SaleResponseDto?> GetSaleByIdAsync(int id)
        {
            _logger.LogInformation("Iniciando busca pela venda {SaleId}", id);
            try{

                var venda = await _saleRepository.GetSaleByIdAsync(id);
                if (venda == null)
                    return null;

                _logger.LogInformation("Venda {SaleId} encontrada com sucesso", id);
                return _mapper.Map<SaleResponseDto>(venda);
            }catch (Exception ex)
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
    }
}
