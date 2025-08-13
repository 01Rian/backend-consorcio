using Bogus;
using Moq;
using rian_p01_back.src.Data.Repositories.Interfaces;
using rian_p01_back.src.Models.Entities;
using rian_p01_back.src.Services.Implementations;

namespace UnitTests.Services
{
    public class CotasServiceTests
    {
        private readonly Mock<ICotasRepository> _mockRepository;
        private readonly CotasService _service;
        private readonly Faker<Cotas> _faker;

        public CotasServiceTests()
        {
            _mockRepository = new Mock<ICotasRepository>();
            _service = new CotasService(_mockRepository.Object);
            _faker = new Faker<Cotas>()
                .RuleFor(c => c.Id, f => f.Random.Int(1, 1000))
                .RuleFor(c => c.NumeroCota, f => f.Random.Replace("####"))
                .RuleFor(c => c.ValorParcela, f => f.Random.Decimal(100, 1000))
                .RuleFor(c => c.ValorPago, f => f.Random.Decimal(0, 5000))
                .RuleFor(c => c.ParcelasPagas, f => f.Random.Int(0, 24))
                .RuleFor(c => c.ConsorcioId, f => f.Random.Int(1, 100))
                .RuleFor(c => c.UsuarioId, f => f.Random.Int(1, 100))
                .RuleFor(c => c.Status, f => StatusCota.Ativo)
                .RuleFor(c => c.Contemplada, f => false)
                .RuleFor(c => c.Ativo, f => true);
        }

        [Fact]
        public async Task GetByConsorcioAsync_ComIdValido_DeveRetornarCotas()
        {
            // Arrange
            var consorcioId = 1;
            var cotas = _faker.Generate(3);
            _mockRepository.Setup(r => r.GetByConsorcioAsync(consorcioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cotas);

            // Act
            var result = await _service.GetByConsorcioAsync(consorcioId);

            // Assert
            Assert.Equal(cotas, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetByConsorcioAsync_ComIdInvalido_DeveLancarArgumentException(int consorcioId)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByConsorcioAsync(consorcioId));
        }

        [Fact]
        public async Task GetByUsuarioAsync_ComIdValido_DeveRetornarCotas()
        {
            // Arrange
            var usuarioId = 1;
            var cotas = _faker.Generate(3);
            _mockRepository.Setup(r => r.GetByUsuarioAsync(usuarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cotas);

            // Act
            var result = await _service.GetByUsuarioAsync(usuarioId);

            // Assert
            Assert.Equal(cotas, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetByUsuarioAsync_ComIdInvalido_DeveLancarArgumentException(int usuarioId)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByUsuarioAsync(usuarioId));
        }

        [Fact]
        public async Task GetDetailedAsync_ComIdValido_DeveRetornarCotaDetalhada()
        {
            // Arrange
            var cota = _faker.Generate();
            _mockRepository.Setup(r => r.GetDetalhadaAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);

            // Act
            var result = await _service.GetDetailedAsync(cota.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cota.Id, result.Id);
        }

        [Fact]
        public async Task GetContemplatedAsync_DeveRetornarApenCotasContempladas()
        {
            // Arrange
            var cotas = _faker.Generate(3);
            cotas.ForEach(c => c.Contemplada = true);
            _mockRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cotas, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cotas);

            // Act
            var result = await _service.GetContemplatedAsync();

            // Assert
            Assert.Equal(cotas, result);
        }

        [Fact]
        public async Task GetActiveAsync_DeveRetornarApenasCotasAtivas()
        {
            // Arrange
            var cotas = _faker.Generate(3);
            cotas.ForEach(c => { c.Ativo = true; c.Status = StatusCota.Ativo; });
            _mockRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cotas, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cotas);

            // Act
            var result = await _service.GetActiveAsync();

            // Assert
            Assert.Equal(cotas, result);
        }

        [Fact]
        public async Task ContemplateAsync_ComCotaValida_DeveContemplarCota()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.Contemplada = false;
            cota.Ativo = true;
            cota.Status = StatusCota.Ativo;
            
            _mockRepository.Setup(r => r.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.ContemplateAsync(cota.Id);

            // Assert
            Assert.True(result);
            Assert.True(cota.Contemplada);
            Assert.Equal(StatusCota.Contemplado, cota.Status);
            Assert.True(cota.DataContemplacao > DateTime.MinValue);
        }

        [Fact]
        public async Task ContemplateAsync_ComCotaJaContemplada_DeveLancarInvalidOperationException()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.Contemplada = true;
            
            _mockRepository.Setup(r => r.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ContemplateAsync(cota.Id));
            Assert.Contains("já está contemplada", exception.Message);
        }

        [Fact]
        public async Task ContemplateAsync_ComCotaInativa_DeveLancarInvalidOperationException()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.Contemplada = false;
            cota.Ativo = false;
            
            _mockRepository.Setup(r => r.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ContemplateAsync(cota.Id));
            Assert.Contains("precisa estar ativa", exception.Message);
        }

        [Fact]
        public async Task ContemplateAsync_ComCotaInexistente_DeveRetornarFalse()
        {
            // Arrange
            var cotaId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(cotaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cotas)null!);

            // Act
            var result = await _service.ContemplateAsync(cotaId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RegisterPaymentAsync_ComValorValido_DeveRegistrarPagamento()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.Ativo = true;
            cota.ValorPago = 1000;
            cota.ParcelasPagas = 5;
            
            var valorPago = 500m;
            
            _mockRepository.Setup(r => r.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.RegisterPaymentAsync(cota.Id, valorPago);

            // Assert
            Assert.True(result);
            Assert.Equal(1500, cota.ValorPago);
            Assert.Equal(6, cota.ParcelasPagas);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task RegisterPaymentAsync_ComValorInvalido_DeveLancarArgumentException(decimal valorPago)
        {
            // Arrange
            var cotaId = 1;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.RegisterPaymentAsync(cotaId, valorPago));
        }

        [Fact]
        public async Task RegisterPaymentAsync_ComCotaInativa_DeveLancarInvalidOperationException()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.Ativo = false;
            
            _mockRepository.Setup(r => r.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterPaymentAsync(cota.Id, 100m));
            Assert.Contains("cota inativa", exception.Message);
        }

        [Fact]
        public async Task RegisterPaymentAsync_ComCotaInexistente_DeveRetornarFalse()
        {
            // Arrange
            var cotaId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(cotaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cotas)null!);

            // Act
            var result = await _service.RegisterPaymentAsync(cotaId, 100m);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CreateAsync_ComCotaValida_DeveCriarCota()
        {
            // Arrange
            var cota = _faker.Generate();
            _mockRepository.Setup(r => r.AddAsync(cota, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(cota);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.DataCadastro > DateTime.MinValue);
            Assert.Equal(StatusCota.Ativo, result.Status);
            Assert.True(result.Ativo);
            Assert.Equal(0, result.ValorPago);
            Assert.Equal(0, result.ParcelasPagas);
            Assert.False(result.Contemplada);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateAsync_ComValorParcelaInvalido_DeveLancarArgumentException(decimal valorParcela)
        {
            // Arrange
            var cota = _faker.Generate();
            cota.ValorParcela = valorParcela;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(cota));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateAsync_ComConsorcioIdInvalido_DeveLancarArgumentException(int consorcioId)
        {
            // Arrange
            var cota = _faker.Generate();
            cota.ConsorcioId = consorcioId;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(cota));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateAsync_ComNumeroCotaInvalido_DeveLancarArgumentException(string numeroCota)
        {
            // Arrange
            var cota = _faker.Generate();
            cota.NumeroCota = numeroCota;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(cota));
        }

        [Fact]
        public async Task CreateAsync_ComNumeroCotaNulo_DeveLancarArgumentException()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.NumeroCota = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(cota));
        }

        [Fact]
        public async Task UpdateAsync_ComCotaValida_DeveAtualizarCota()
        {
            // Arrange
            var cota = _faker.Generate();
            _mockRepository.Setup(r => r.Update(cota));
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(cota);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.DataAtualizacao > DateTime.MinValue);
        }
    }
}
