using AutoMapper;
using ViberLounge.API.Controllers;
using ViberLounge.Domain.Entities;
using ViberLounge.Application.DTOs.Product;

namespace ViberLounge.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Produto, ProductDto>();
            CreateMap<CreateProductDto, Produto>();
            // CreateMap<CriarProdutoCommand, Produto>();
            // CreateMap<AtualizarProdutoCommand, Produto>();
            CreateMap<Venda, VendaDto>();
            CreateMap<VendaItem, ItemVendaDto>();
            CreateMap<Usuario, UsuarioDto>();
        }
    }
}
