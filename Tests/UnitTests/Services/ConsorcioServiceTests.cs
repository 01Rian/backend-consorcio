using Bogus;
using Moq;
using rian_p01_back.src.Data.Repositories.Interfaces;
using rian_p01_back.src.Models.Entities;
using rian_p01_back.src.Services.Implementations;

namespace UnitTests.Services
{
    public class ConsorcioServiceTests
    {
        private readonly Mock<IConsorcioRepository> _mockRepository;
        private readonly ConsorcioService _service;
        private readonly Faker<Consorcio> _faker;

        public ConsorcioServiceTests()
        {
            _mockRepository = new Mock<IConsorcioRepository>();
            _service = new ConsorcioService(_mockRepository.Object);
            _faker = new Faker<Consorcio>()
                .RuleFor(c => c.Id, f => f.Random.Int(1, 1000))
                .RuleFor(c => c.Nome, f => f.Commerce.ProductName())
                .RuleFor(c => c.ValorBem, f => f.Random.Decimal(10000, 100000))
                .RuleFor(c => c.QuantidadeCotas, f => f.Random.Int(10, 100))
                .RuleFor(c => c.PrazoMeses, f => f.Random.Int(12, 60))
                .RuleFor(c => c.TaxaAdministracao, f => f.Random.Decimal(0, 10))
                .RuleFor(c => c.FundoReserva, f => f.Random.Decimal(0, 5000))
                .RuleFor(c => c.DataInicio, f => f.Date.Past())
                .RuleFor(c => c.DataTermino, f => f.Date.Future())
                .RuleFor(c => c.Ativo, f => f.Random.Bool());
        }

        [Fact]
        public async Task GetWithCotasAsync_ComIdValido_DeveRetornarConsorcioComCotas()
        {
            // Arrange
            var consorcio = _faker.Generate();
            _mockRepository.Setup(r => r.GetWithCotasAsync(consorcio.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(consorcio);

            // Act
            var result = await _service.GetWithCotasAsync(consorcio.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(consorcio.Id, result.Id);
        }

        [Fact]
        public async Task GetActiveAsync_DeveRetornarApenasConsorciosAtivos()
        {
            // Arrange
            var consorcios = _faker.Generate(3);
            consorcios.ForEach(c => c.Ativo = true);
            _mockRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Consorcio, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(consorcios);

            // Act
            var result = await _service.GetActiveAsync();

            // Assert
            Assert.Equal(consorcios, result);
        }

        [Fact]
        public async Task CreateAsync_ComConsorcioValido_DeveCriarConsorcio()
        {
            // Arrange
            var consorcio = _faker.Generate();
            _mockRepository.Setup(r => r.AddAsync(consorcio, It.IsAny<CancellationToken>()))
                .ReturnsAsync(consorcio);
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(consorcio);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.DataCadastro > DateTime.MinValue);
            Assert.True(result.Ativo);
            Assert.Equal(consorcio.DataInicio.AddMonths(consorcio.PrazoMeses).Date, result.DataTermino.Date);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateAsync_ComValorBemInvalido_DeveLancarArgumentException(decimal valorBem)
        {
            // Arrange
            var consorcio = _faker.Generate();
            consorcio.ValorBem = valorBem;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(consorcio));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateAsync_ComQuantidadeCotasInvalida_DeveLancarArgumentException(int quantidadeCotas)
        {
            // Arrange
            var consorcio = _faker.Generate();
            consorcio.QuantidadeCotas = quantidadeCotas;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(consorcio));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateAsync_ComPrazoMesesInvalido_DeveLancarArgumentException(int prazoMeses)
        {
            // Arrange
            var consorcio = _faker.Generate();
            consorcio.PrazoMeses = prazoMeses;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(consorcio));
        }

        [Fact]
        public async Task CreateAsync_ComTaxaAdministracaoNegativa_DeveLancarArgumentException()
        {
            // Arrange
            var consorcio = _faker.Generate();
            consorcio.TaxaAdministracao = -1;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(consorcio));
        }

        [Fact]
        public async Task CreateAsync_ComFundoReservaNegativo_DeveLancarArgumentException()
        {
            // Arrange
            var consorcio = _faker.Generate();
            consorcio.FundoReserva = -1;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(consorcio));
        }

        [Fact]
        public async Task UpdateAsync_ComConsorcioValido_DeveAtualizarConsorcio()
        {
            // Arrange
            var consorcio = _faker.Generate();
            _mockRepository.Setup(r => r.Update(consorcio));
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(consorcio);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.DataAtualizacao > DateTime.MinValue);
            Assert.Equal(consorcio.DataInicio.AddMonths(consorcio.PrazoMeses).Date, result.DataTermino.Date);
        }
    }
}
