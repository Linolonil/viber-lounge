using AutoMapper;
using ViberLounge.Application.Mapping;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Repositories.Interfaces;

namespace ViberLounge.Tests.Unit.Application.ProductServiceTests
{
    public class GetProductByTermTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
        private readonly Mock<ILoggerService> _loggerMock;

        public GetProductByTermTest()
        {
            _produtoRepositoryMock = new Mock<IProdutoRepository>();
            _loggerMock = new Mock<ILoggerService>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }
    }
}