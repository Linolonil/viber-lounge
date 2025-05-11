using ViberLounge.Domain.Entities;
using ViberLounge.Application.DTOs.Sale;
using ViberLounge.Application.Services.Interfaces;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Application.Services
{
    public class VendaService : IVendaService
    {
        private readonly IVendaRepository _saleRepository;
        private readonly IUsuarioRepository _userRepository;
        private readonly IProdutoRepository _productRepository;
        public VendaService(IVendaRepository saleRepository, IUsuarioRepository userRepository, IProdutoRepository productRepository)
        {
            _saleRepository = saleRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        public async Task<CreateSaleDto> CreateAsync(CreateSaleDto saleDto)
        {
            if (saleDto.Items == null || saleDto.Items.Count == 0)
                throw new Exception("Nenhum item informado na venda");

            bool userExists = await _userRepository.UserExistsAsync(saleDto.UserId);
            if (!userExists)
                throw new Exception("Usuário não encontrado");

            var sale = new Venda
            {
                NomeCliente = saleDto.CustomerName ?? "NAO_INFORMADO",
                IdUsuario = saleDto.UserId,
                PrecoTotal = Convert.ToDouble(saleDto.TotalPrice!),
                FormaPagamento = saleDto.PaymentType
            };

            var saleItems = new List<VendaItem>();
            
            foreach (var item in saleDto.Items)
            {
                var product = await _productRepository.GetProductByIdAndAvailableStatus(item.ProductId);
                if (product == null || product!.Quantidade < item.Quantity)
                    continue;

                saleItems.Add(new VendaItem
                {
                    IdProduto = item.ProductId,
                    Quantidade = item.Quantity,
                    Subtotal = Convert.ToDouble(item.Subtotal)
                });
            }

            if (saleItems.Count == 0)
                throw new Exception("Nenhum item válido para salvar.");

            bool sucesso = await _saleRepository.CreateSaleWithItemsAsync(sale, saleItems);

            if (!sucesso)
                throw new Exception("Erro ao criar venda e seus itens.");

            return saleDto;
        }
    }
}
