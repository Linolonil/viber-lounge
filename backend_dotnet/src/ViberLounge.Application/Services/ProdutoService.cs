using MediatR;
using ViberLounge.API.Controllers;
using ViberLounge.Application.Services.Interfaces;

namespace ViberLounge.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IMediator _mediator;

        public ProdutoService(IMediator mediator)
        {
            _mediator = mediator;
        }

        // public async Task<ProdutoDto> GetByIdAsync(int id)
        // {
        //     return await _mediator.Send(new GetProdutoByIdQuery { Id = id });
        // }

        // public async Task<List<ProdutoDto>> GetAllAsync()
        // {
        //     return await _mediator.Send(new GetAllProdutosQuery());
        // }

        // public async Task<ProdutoDto> CreateAsync(CriarProdutoCommand command)
        // {
        //     return await _mediator.Send(command);
        // }

        // public async Task<ProdutoDto> UpdateAsync(AtualizarProdutoCommand command)
        // {
        //     return await _mediator.Send(command);
        // }

        // public async Task DeleteAsync(int id)
        // {
        //     await _mediator.Send(new DeletarProdutoCommand { Id = id });
        // }

        Task<ProdutoDto> IProdutoService.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<List<ProdutoDto>> IProdutoService.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
