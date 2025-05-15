using AutoMapper;
using ViberLounge.API.Controllers;
using ViberLounge.Domain.Entities;
using ViberLounge.Application.DTOs.Sale;
using ViberLounge.Application.DTOs.Product;

namespace ViberLounge.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Usuario, UserDto>();
            CreateMap<Produto, ProductDto>();
            CreateMap<CreateProductDto, Produto>();

            CreateMap<Venda, SaleResponseDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.NomeCliente))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.PrecoTotal))
                .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.FormaPagamento))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Itens));

            CreateMap<VendaItem, SaleItemResponseFromData>()
                .ForMember(dest => dest.IdSaleItem, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.IdProduto))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantidade))
                .ForMember(dest => dest.TotalItemPrice, opt => opt.MapFrom(src => src.Subtotal))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

            // Mapeamento da venda
            CreateMap<Venda, SaleResponseFromDataDto>()
                .ForMember(dest => dest.IdSale, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.NomeCliente))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(dest => dest.EmployeName, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Nome : null))
                .ForMember(dest => dest.TotalSalePrice, opt => opt.MapFrom(src => src.PrecoTotal))
                .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.FormaPagamento))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Itens));
        }
    }
}
