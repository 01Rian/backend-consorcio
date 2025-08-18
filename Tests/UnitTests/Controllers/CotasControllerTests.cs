using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
using rian_p01_back.src.Models.Entities;
using rian_p01_back.src.Services.Interfaces;
using rian_p01_back.Src.Controllers.Cotas;

namespace UnitTests.Controllers
{
    public class CotasControllerTests
    {
        private readonly Mock<ICotasService> _mockCotasService;
        private readonly CotasController _controller;
        private readonly Faker<Cotas> _cotasFaker;
        private readonly Faker<RegisterPaymentRequestDto> _registerPaymentRequestFaker;

        public CotasControllerTests()
        {
            _mockCotasService = new Mock<ICotasService>();
            _controller = new CotasController(_mockCotasService.Object);

            _cotasFaker = new Faker<Cotas>()
                .RuleFor(c => c.Id, f => f.Random.Int(1, 1000))
                .RuleFor(c => c.NumeroCota, f => f.Random.AlphaNumeric(10).ToUpper())
                .RuleFor(c => c.ValorParcela, f => f.Random.Decimal(100, 5000))
                .RuleFor(c => c.ValorPago, f => f.Random.Decimal(0, 2500))
                .RuleFor(c => c.Status, f => f.PickRandom<StatusCota>())
                .RuleFor(c => c.ConsorcioId, f => f.Random.Int(1, 100))
                .RuleFor(c => c.UsuarioId, f => f.Random.Int(1, 100))
                .RuleFor(c => c.DataCadastro, f => f.Date.Past())
                .RuleFor(c => c.DataAtualizacao, f => f.Date.Recent())
                .RuleFor(c => c.DataContemplacao, f => f.Date.Recent())
                .RuleFor(c => c.Ativo, f => true);

            _registerPaymentRequestFaker = new Faker<RegisterPaymentRequestDto>()
                .RuleFor(r => r.ValorPago, f => f.Random.Decimal(0.01m, 5000m));
        }

        #region GetById Tests

        [Fact]
        public async Task GetById_ComIdValido_DeveRetornarOk()
        {
            // Arrange
            var cota = _cotasFaker.Generate();
            var id = cota.Id;

            _mockCotasService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);

            // Act
            var result = await _controller.GetById(id, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_ComIdInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var id = 0;

            // Act
            var result = await _controller.GetById(id, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("ID deve ser maior que zero", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task GetById_ComCotaNaoEncontrada_DeveRetornarNotFound()
        {
            // Arrange
            var id = 999;

            _mockCotasService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cotas?)null);

            // Act
            var result = await _controller.GetById(id, CancellationToken.None);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("Cota não encontrada", notFoundResult.Value?.ToString());
        }

        #endregion

        #region GetDetailed Tests

        [Fact]
        public async Task GetDetailed_ComIdValido_DeveRetornarOk()
        {
            // Arrange
            var cota = _cotasFaker.Generate();
            var id = cota.Id;

            _mockCotasService.Setup(s => s.GetDetailedAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cota);

            // Act
            var result = await _controller.GetDetailed(id, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetDetailed_ComIdInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var id = 0;

            // Act
            var result = await _controller.GetDetailed(id, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("ID deve ser maior que zero", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task GetDetailed_ComCotaNaoEncontrada_DeveRetornarNotFound()
        {
            // Arrange
            var id = 999;

            _mockCotasService.Setup(s => s.GetDetailedAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cotas?)null);

            // Act
            var result = await _controller.GetDetailed(id, CancellationToken.None);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("Cota não encontrada", notFoundResult.Value?.ToString());
        }

        #endregion

        #region GetByConsorcio Tests

        [Fact]
        public async Task GetByConsorcio_ComIdValido_DeveRetornarOk()
        {
            // Arrange
            var consorcioId = 1;
            var cotas = _cotasFaker.Generate(3);

            _mockCotasService.Setup(s => s.GetByConsorcioAsync(consorcioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cotas);

            // Act
            var result = await _controller.GetByConsorcio(consorcioId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetByConsorcio_ComIdInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var consorcioId = 0;

            // Act
            var result = await _controller.GetByConsorcio(consorcioId, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("ID do consórcio deve ser maior que zero", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task GetByConsorcio_ComArgumentException_DeveLancarArgumentException()
        {
            // Arrange
            var consorcioId = 1;
            var errorMessage = "Dados inválidos";

            _mockCotasService.Setup(s => s.GetByConsorcioAsync(consorcioId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.GetByConsorcio(consorcioId, CancellationToken.None));
            Assert.Equal(errorMessage, exception.Message);
        }

        #endregion

        #region GetByUsuario Tests

        [Fact]
        public async Task GetByUsuario_ComIdValido_DeveRetornarOk()
        {
            // Arrange
            var usuarioId = 1;
            var cotas = _cotasFaker.Generate(2);

            _mockCotasService.Setup(s => s.GetByUsuarioAsync(usuarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cotas);

            // Act
            var result = await _controller.GetByUsuario(usuarioId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetByUsuario_ComIdInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var usuarioId = 0;

            // Act
            var result = await _controller.GetByUsuario(usuarioId, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("ID do usuário deve ser maior que zero", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task GetByUsuario_ComArgumentException_DeveLancarArgumentException()
        {
            // Arrange
            var usuarioId = 1;
            var errorMessage = "Dados inválidos";

            _mockCotasService.Setup(s => s.GetByUsuarioAsync(usuarioId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.GetByUsuario(usuarioId, CancellationToken.None));
            Assert.Equal(errorMessage, exception.Message);
        }

        #endregion

        #region GetActive Tests

        [Fact]
        public async Task GetActive_DeveRetornarOkComListaDeCotas()
        {
            // Arrange
            var cotas = _cotasFaker.Generate(3);

            _mockCotasService.Setup(s => s.GetActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(cotas);

            // Act
            var result = await _controller.GetActive(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        #endregion

        #region GetContemplated Tests

        [Fact]
        public async Task GetContemplated_DeveRetornarOkComListaDeCotasContempladas()
        {
            // Arrange
            var cotas = _cotasFaker.Generate(2);

            _mockCotasService.Setup(s => s.GetContemplatedAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(cotas);

            // Act
            var result = await _controller.GetContemplated(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        #endregion

        #region GetAvailableCotas Tests

        #endregion

        #region Contemplate Tests

        [Fact]
        public async Task Contemplate_ComIdValido_DeveRetornarOk()
        {
            // Arrange
            var id = 1;
            var cota = new Cotas
            {
                Id = id,
                Contemplada = false,
                Ativo = true,
                Status = StatusCota.Ativo
            };

            _mockCotasService.Setup(s => s.ContemplateAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Contemplate(id, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Cota contemplada com sucesso", okResult.Value?.ToString());
        }

        [Fact]
        public async Task Contemplate_ComIdInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var id = 0;

            // Act
            var result = await _controller.Contemplate(id, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("ID deve ser maior que zero", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task Contemplate_ComCotaNaoEncontrada_DeveRetornarNotFound()
        {
            // Arrange
            var id = 999;

            _mockCotasService.Setup(s => s.ContemplateAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Contemplate(id, CancellationToken.None);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Cota não encontrada", notFoundResult.Value?.ToString());
        }

        [Fact]
    public async Task Contemplate_ComCotaJaContemplada_DeveLancarInvalidOperationException()
        {
            // Arrange
            var id = 1;
            var cota = new Cotas
            {
                Id = id,
                Contemplada = true,
                Ativo = true,
                Status = StatusCota.Contemplado
            };

            _mockCotasService.Setup(s => s.ContemplateAsync(id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("A cota já está contemplada"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Contemplate(id, CancellationToken.None));
            Assert.Equal("A cota já está contemplada", exception.Message);
        }

        [Fact]
    public async Task Contemplate_ComCotaInativa_DeveLancarInvalidOperationException()
        {
            // Arrange
            var id = 1;
            var cota = new Cotas
            {
                Id = id,
                Contemplada = false,
                Ativo = false,
                Status = StatusCota.Suspenso
            };

            _mockCotasService.Setup(s => s.ContemplateAsync(id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("A cota precisa estar ativa para ser contemplada"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Contemplate(id, CancellationToken.None));
            Assert.Equal("A cota precisa estar ativa para ser contemplada", exception.Message);
        }

        #endregion

        #region RegisterPayment Tests

        [Fact]
        public async Task RegisterPayment_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var id = 1;
            var request = _registerPaymentRequestFaker.Generate();

            _mockCotasService.Setup(s => s.RegisterPaymentAsync(id, request.ValorPago, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.RegisterPayment(id, request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Pagamento registrado com sucesso", okResult.Value?.ToString());
        }

        [Fact]
        public async Task RegisterPayment_ComIdInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var id = 0;
            var request = _registerPaymentRequestFaker.Generate();

            // Act
            var result = await _controller.RegisterPayment(id, request, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("ID deve ser maior que zero", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task RegisterPayment_ComCotaNaoEncontrada_DeveRetornarNotFound()
        {
            // Arrange
            var id = 999;
            var request = _registerPaymentRequestFaker.Generate();

            _mockCotasService.Setup(s => s.RegisterPaymentAsync(id, request.ValorPago, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.RegisterPayment(id, request, CancellationToken.None);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Cota não encontrada", notFoundResult.Value?.ToString());
        }

        [Fact]
        public async Task RegisterPayment_ComArgumentException_DeveLancarArgumentException()
        {
            // Arrange
            var id = 1;
            var request = _registerPaymentRequestFaker.Generate();
            var errorMessage = "Valor inválido";

            _mockCotasService.Setup(s => s.RegisterPaymentAsync(id, request.ValorPago, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.RegisterPayment(id, request, CancellationToken.None));
            Assert.Equal(errorMessage, exception.Message);
        }

        [Fact]
        public async Task RegisterPayment_ComInvalidOperationException_DeveLancarInvalidOperationException()
        {
            // Arrange
            var id = 1;
            var request = _registerPaymentRequestFaker.Generate();
            var errorMessage = "Cota inativa";

            _mockCotasService.Setup(s => s.RegisterPaymentAsync(id, request.ValorPago, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(errorMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.RegisterPayment(id, request, CancellationToken.None));
            Assert.Equal(errorMessage, exception.Message);
        }

        #endregion

        #region RemoveUsuario Tests

        [Fact]
        public async Task RemoveUsuario_ComIdValido_DeveRetornarOk()
        {
            // Arrange
            var id = 1;

            _mockCotasService.Setup(s => s.RemoveUsuarioFromCotaAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveUser(id, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Usuário removido da cota com sucesso", okResult.Value?.ToString());
        }

        [Fact]
        public async Task RemoveUsuario_ComIdInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var id = 0;

            // Act
            var result = await _controller.RemoveUser(id, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("ID deve ser maior que zero", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task RemoveUsuario_ComCotaNaoEncontrada_DeveRetornarNotFound()
        {
            // Arrange
            var id = 999;

            _mockCotasService.Setup(s => s.RemoveUsuarioFromCotaAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.RemoveUser(id, CancellationToken.None);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Cota não encontrada", notFoundResult.Value?.ToString());
        }

        [Fact]
        public async Task RemoveUsuario_ComInvalidOperationException_DeveLancarInvalidOperationException()
        {
            // Arrange
            var id = 1;
            var errorMessage = "Cota tem pagamentos";

            _mockCotasService.Setup(s => s.RemoveUsuarioFromCotaAsync(id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(errorMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.RemoveUser(id, CancellationToken.None));
            Assert.Equal(errorMessage, exception.Message);
        }

        #endregion
    }
}
