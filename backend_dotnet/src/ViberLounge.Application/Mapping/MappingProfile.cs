namespace ViberLounge.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Produto, ProdutoDto>();
            CreateMap<CriarProdutoCommand, Produto>();
            CreateMap<AtualizarProdutoCommand, Produto>();
            CreateMap<Venda, VendaDto>();
            CreateMap<ItemVenda, ItemVendaDto>();
            CreateMap<Usuario, UsuarioDto>();
        }
    }
}
