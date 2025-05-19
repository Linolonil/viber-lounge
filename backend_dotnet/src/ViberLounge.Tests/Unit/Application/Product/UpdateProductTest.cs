using AutoMapper;
using ViberLounge.Application.Mapping;
using ViberLounge.Infrastructure.Logging;
using ViberLounge.Infrastructure.Repositories.Interfaces;
using ViberLounge.Infrastructure.Services;

namespace ViberLounge.Tests.Unit.Application.ProductServiceTests
{
    public class UpdateProductTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly Mock<IFileService> _fileServiceMock;

        public UpdateProductTest()
        {
            _produtoRepositoryMock = new Mock<IProdutoRepository>();
            _loggerMock = new Mock<ILoggerService>();
            _fileServiceMock = new Mock<IFileService>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }
    }
}