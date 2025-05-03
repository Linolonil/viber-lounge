using AutoMapper;
using ViberLounge.API.Controllers;
using ViberLounge.Domain.Entities;

namespace ViberLounge.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Produto, ProdutoDto>();
            // CreateMap<CriarProdutoCommand, Produto>();
            // CreateMap<AtualizarProdutoCommand, Produto>();
            CreateMap<Venda, VendaDto>();
            CreateMap<ItemVenda, ItemVendaDto>();
            CreateMap<Usuario, UsuarioDto>();
        }
    }
}
