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
        public async Task RegisterPaymentAsync_ComValorValido_DeveRegistrarPagamento()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.Ativo = true;
            cota.ValorParcela = 500m;
            cota.ValorPago = 1000m;
            cota.ParcelasPagas = 5;

            var valorPago = 500m; // corresponde a 1 parcela

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

        [Fact]
        public async Task RegisterPaymentAsync_ComPagamentoMenorQueParcela_DeveLancarInvalidOperationException()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.Ativo = true;
            cota.ValorParcela = 500m;
            cota.ValorPago = 0m;
            cota.ParcelasPagas = 0;

            var valorPago = 100m; // menor que a parcela

            _mockRepository.Setup(r => r.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterPaymentAsync(cota.Id, valorPago));
            Assert.Contains("não pode ser menor que o valor da parcela", exception.Message);
        }

        [Fact]
        public async Task RegisterPaymentAsync_ComPagamentoMaiorMasNaoMultiplo_DeveLancarInvalidOperationException()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.Ativo = true;
            cota.ValorParcela = 500m;
            cota.ValorPago = 0m;
            cota.ParcelasPagas = 0;

            var valorPago = 750m; // maior que a parcela, mas não múltiplo exato

            _mockRepository.Setup(r => r.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterPaymentAsync(cota.Id, valorPago));
            Assert.Contains("Pagamento inválido", exception.Message);
        }

        [Fact]
        public async Task RegisterPaymentAsync_ComPagamentoMultiploDeVariasParcelas_DeveRegistrarPagamento()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.Ativo = true;
            cota.ValorParcela = 500m;
            cota.ValorPago = 1000m; // já pagou 2 parcelas
            cota.ParcelasPagas = 2;

            var valorPago = 1500m; // quer pagar 3 parcelas adicionais (1500 / 500 = 3)

            _mockRepository.Setup(r => r.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.RegisterPaymentAsync(cota.Id, valorPago);

            // Assert
            Assert.True(result);
            Assert.Equal(2500m, cota.ValorPago); // 1000 + 1500
            Assert.Equal(5, cota.ParcelasPagas); // 2 + 3
        }

        [Fact]
        public async Task RegisterPaymentAsync_ExcedeParcelasRestantes_DeveLancarInvalidOperationException()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.Ativo = true;
            cota.ValorParcela = 500m;
            cota.ValorPago = 0m;
            cota.ParcelasPagas = 4; // já pagou 4 parcelas

            var valorPago = 1500m; // tenta pagar 3 parcelas

            // Consorcio com prazo de 6 meses -> restam 2 parcelas
            var primeira = new Cotas { Consorcio = new Consorcio { PrazoMeses = 6 } };

            _mockRepository.Setup(r => r.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);
            _mockRepository.Setup(r => r.GetByConsorcioAsync(cota.ConsorcioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Cotas> { primeira });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterPaymentAsync(cota.Id, valorPago));
            Assert.Contains("Não é possível pagar", exception.Message);
        }

        [Fact]
        public async Task RegisterPaymentAsync_ComParcelasRestantesZero_DeveLancarInvalidOperationException()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.Ativo = true;
            cota.ValorParcela = 500m;
            cota.ValorPago = 3000m;
            cota.ParcelasPagas = 6; // igual ao prazo

            var valorPago = 500m; // qualquer valor

            var primeira = new Cotas { Consorcio = new Consorcio { PrazoMeses = 6 } };

            _mockRepository.Setup(r => r.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);
            _mockRepository.Setup(r => r.GetByConsorcioAsync(cota.ConsorcioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Cotas> { primeira });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterPaymentAsync(cota.Id, valorPago));
            Assert.Contains("já está quitada", exception.Message);
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

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task RemoveUsuarioFromCotaAsync_ComCotaIdInvalido_DeveLancarArgumentException(int cotaId)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.RemoveUsuarioFromCotaAsync(cotaId));
        }

        [Fact]
        public async Task RemoveUsuarioFromCotaAsync_ComCotaInexistente_DeveRetornarFalse()
        {
            // Arrange
            var cotaId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(cotaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cotas)null!);

            // Act
            var result = await _service.RemoveUsuarioFromCotaAsync(cotaId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RemoveUsuarioFromCotaAsync_ComCotaSemUsuario_DeveLancarInvalidOperationException()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.UsuarioId = null;
            _mockRepository.Setup(r => r.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoveUsuarioFromCotaAsync(cota.Id));
        }

        [Fact]
        public async Task RemoveUsuarioFromCotaAsync_ComCotaComPagamentos_DeveLancarInvalidOperationException()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.UsuarioId = 5;
            cota.ValorPago = 100;
            cota.ParcelasPagas = 1;
            _mockRepository.Setup(r => r.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoveUsuarioFromCotaAsync(cota.Id));
        }

        [Fact]
        public async Task RemoveUsuarioFromCotaAsync_ComCotaValida_DeveRemoverUsuario()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.UsuarioId = 5;
            cota.ValorPago = 0;
            cota.ParcelasPagas = 0;
            _mockRepository.Setup(r => r.GetByIdAsync(cota.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.RemoveUsuarioFromCotaAsync(cota.Id);

            // Assert
            Assert.True(result);
            Assert.Null(cota.UsuarioId);
            _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
