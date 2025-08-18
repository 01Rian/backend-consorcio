using Bogus;
using Moq;
using rian_p01_back.src.Data.Repositories.Interfaces;
using rian_p01_back.src.Models.Entities;
using rian_p01_back.src.Services.Implementations;
using rian_p01_back.src.Services.Interfaces;

namespace UnitTests.Services
{
    public class ConsorcioServiceTests
    {
        private readonly Mock<IConsorcioRepository> _mockRepository;
        private readonly Mock<ICotasService> _mockCotasService;
        private readonly ConsorcioService _service;
        private readonly Faker<Consorcio> _faker;

        public ConsorcioServiceTests()
        {
            _mockRepository = new Mock<IConsorcioRepository>();
            _mockCotasService = new Mock<ICotasService>();
            _service = new ConsorcioService(_mockRepository.Object, _mockCotasService.Object);
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

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task AssignCotaToUsuarioAsync_ComCotaIdInvalido_DeveLancarArgumentException(int cotaId)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.AssignCotaToUsuarioAsync(cotaId, 1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task AssignCotaToUsuarioAsync_ComUsuarioIdInvalido_DeveLancarArgumentException(int usuarioId)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.AssignCotaToUsuarioAsync(1, usuarioId));
        }

        [Fact]
        public async Task AssignCotaToUsuarioAsync_ComCotaInexistente_DeveLancarArgumentException()
        {
            // Arrange
            var cotaId = 999;
            _mockCotasService.Setup(c => c.GetByIdAsync(cotaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cotas)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AssignCotaToUsuarioAsync(cotaId, 1));
            Assert.Contains("Cota", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AssignCotaToUsuarioAsync_ComCotaJaAtribuida_DeveReatribuirParaOutroUsuario()
        {
            // Arrange
            var cota = new Cotas { Id = 1, UsuarioId = 10 };
            _mockCotasService.Setup(c => c.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);
            _mockCotasService.Setup(c => c.UpdateAsync(It.IsAny<Cotas>(), It.IsAny<CancellationToken>()))
                .Returns((Cotas updated, CancellationToken ct) => Task.FromResult(updated));

            // Act
            var result = await _service.AssignCotaToUsuarioAsync(cota.Id, 2);

            // Assert
            Assert.True(result);
            Assert.Equal(2, cota.UsuarioId);
            _mockCotasService.Verify(m => m.UpdateAsync(It.Is<Cotas>(x => x.Id == cota.Id && x.UsuarioId == 2), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AssignCotaToUsuarioAsync_ComDadosValidos_DeveAtribuirCotaAoUsuario()
        {
            // Arrange
            var cota = new Cotas { Id = 1, UsuarioId = null };
            _mockCotasService.Setup(c => c.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);
            _mockCotasService.Setup(c => c.UpdateAsync(It.IsAny<Cotas>(), It.IsAny<CancellationToken>()))
                .Returns((Cotas updated, CancellationToken ct) => Task.FromResult(updated));

            // Act
            var result = await _service.AssignCotaToUsuarioAsync(cota.Id, 5);

            // Assert
            Assert.True(result);
            Assert.Equal(5, cota.UsuarioId);
            _mockCotasService.Verify(m => m.UpdateAsync(It.Is<Cotas>(x => x.Id == cota.Id && x.UsuarioId == 5), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetAvailableCotasAsync_ComConsorcioIdInvalido_DeveLancarArgumentException(int consorcioId)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetAvailableCotasAsync(consorcioId));
        }

        [Fact]
        public async Task GetAvailableCotasAsync_ComConsorcioValido_DeveRetornarCotasDisponiveis()
        {
            // Arrange
            var consorcioId = 1;
            var cotas = new List<Cotas>
            {
                new Cotas { Id = 1, UsuarioId = null, Ativo = true, Status = StatusCota.Ativo },
                new Cotas { Id = 2, UsuarioId = 5, Ativo = true, Status = StatusCota.Ativo },
                new Cotas { Id = 3, UsuarioId = null, Ativo = false, Status = StatusCota.Ativo },
                new Cotas { Id = 4, UsuarioId = null, Ativo = true, Status = StatusCota.Suspenso }
            };

            _mockCotasService.Setup(c => c.GetByConsorcioAsync(consorcioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cotas);

            // Act
            var result = (await _service.GetAvailableCotasAsync(consorcioId)).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
        }

        [Fact]
        public async Task CreateAsync_ComQuantidadeCotas_DeveCriarApenasQuantidadeDefinida()
        {
            // Arrange
            var consorcio = _faker.Generate();
            consorcio.QuantidadeCotas = 1;
            consorcio.Cotas = new List<Cotas>();

            _mockRepository.Setup(r => r.AddAsync(consorcio, It.IsAny<CancellationToken>())).ReturnsAsync(consorcio);
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var createdCotas = new List<Cotas>();
            _mockCotasService.Setup(c => c.CreateAsync(It.IsAny<Cotas>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cotas c, CancellationToken ct) => { createdCotas.Add(c); return c; });

            // Act
            var result = await _service.CreateAsync(consorcio);

            // Assert
            _mockCotasService.Verify(c => c.CreateAsync(It.IsAny<Cotas>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
            Assert.Single(createdCotas);
            Assert.Equal(consorcio.Codigo, createdCotas[0].NumeroCota.Split('-')[0]);
            Assert.Equal(1, result.QuantidadeCotas);
        }

        [Fact]
        public async Task CreateAsync_SemDataInicio_DeveLancarArgumentException()
        {
            // Arrange
            var consorcio = _faker.Generate();
            consorcio.DataInicio = DateTime.MinValue;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(consorcio));
        }
    }
}
