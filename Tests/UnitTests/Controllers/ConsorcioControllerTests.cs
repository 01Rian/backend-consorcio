using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
using rian_p01_back.src.Models.Entities;
using rian_p01_back.src.Services.Interfaces;
using rian_p01_back.src.Controllers.Consorcio;

namespace UnitTests.Controllers
{
    public class ConsorcioControllerTests
    {
        private readonly Mock<IConsorcioService> _mockConsorcioService;
        private readonly ConsorcioController _controller;
        private readonly Faker<Consorcio> _consorcioFaker;
        private readonly Faker<CreateConsorcioRequestDto> _createRequestFaker;
        private readonly Faker<UpdateConsorcioRequestDto> _updateRequestFaker;
        private readonly Faker<AddCotasRequestDto> _addCotasRequestFaker;
        private readonly Faker<AssignCotaRequestDto> _assignCotaRequestFaker;

        public ConsorcioControllerTests()
        {
            _mockConsorcioService = new Mock<IConsorcioService>();
            _controller = new ConsorcioController(_mockConsorcioService.Object);

            _consorcioFaker = new Faker<Consorcio>()
                .RuleFor(c => c.Id, f => f.Random.Int(1, 1000))
                .RuleFor(c => c.Nome, f => f.Company.CompanyName())
                .RuleFor(c => c.Codigo, f => f.Random.AlphaNumeric(10).ToUpper())
                .RuleFor(c => c.ValorBem, f => f.Random.Decimal(10000, 500000))
                .RuleFor(c => c.QuantidadeCotas, f => f.Random.Int(10, 100))
                .RuleFor(c => c.PrazoMeses, f => f.Random.Int(12, 120))
                .RuleFor(c => c.TaxaAdministracao, f => f.Random.Decimal(0.01m, 0.20m))
                .RuleFor(c => c.FundoReserva, f => f.Random.Decimal(0.01m, 0.10m))
                .RuleFor(c => c.DataInicio, f => f.Date.Future())
                .RuleFor(c => c.DataTermino, f => f.Date.Future())
                .RuleFor(c => c.Ativo, f => true)
                .RuleFor(c => c.DataCadastro, f => f.Date.Past())
                .RuleFor(c => c.DataAtualizacao, f => f.Date.Recent());

            _createRequestFaker = new Faker<CreateConsorcioRequestDto>()
                .RuleFor(r => r.Nome, f => f.Company.CompanyName())
                .RuleFor(r => r.Codigo, f => f.Random.AlphaNumeric(10).ToUpper())
                .RuleFor(r => r.ValorBem, f => f.Random.Decimal(10000, 500000))
                .RuleFor(r => r.QuantidadeCotas, f => f.Random.Int(10, 100))
                .RuleFor(r => r.PrazoMeses, f => f.Random.Int(12, 120))
                .RuleFor(r => r.TaxaAdministracao, f => f.Random.Decimal(0.01m, 0.20m))
                .RuleFor(r => r.FundoReserva, f => f.Random.Decimal(0.01m, 0.10m))
                .RuleFor(r => r.DataInicio, f => f.Date.Future())
                .RuleFor(r => r.Ativo, f => true);

            _updateRequestFaker = new Faker<UpdateConsorcioRequestDto>()
                .RuleFor(r => r.Nome, f => f.Company.CompanyName())
                .RuleFor(r => r.Codigo, f => f.Random.AlphaNumeric(10).ToUpper())
                .RuleFor(r => r.ValorBem, f => f.Random.Decimal(10000, 500000))
                .RuleFor(r => r.PrazoMeses, f => f.Random.Int(12, 120))
                .RuleFor(r => r.TaxaAdministracao, f => f.Random.Decimal(0.01m, 0.20m))
                .RuleFor(r => r.FundoReserva, f => f.Random.Decimal(0.01m, 0.10m))
                .RuleFor(r => r.Ativo, f => true);

            _addCotasRequestFaker = new Faker<AddCotasRequestDto>()
                .RuleFor(r => r.Quantidade, f => f.Random.Int(1, 50));

            _assignCotaRequestFaker = new Faker<AssignCotaRequestDto>()
                .RuleFor(r => r.CotaId, f => f.Random.Int(1, 1000))
                .RuleFor(r => r.UsuarioId, f => f.Random.Int(1, 1000));
        }

        #region Create Tests

        [Fact]
        public async Task Create_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var request = _createRequestFaker.Generate();
            var consorcio = _consorcioFaker.Generate();

            _mockConsorcioService.Setup(s => s.CreateAsync(It.IsAny<Consorcio>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(consorcio);

            // Act
            var result = await _controller.Create(request, CancellationToken.None);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            Assert.NotNull(createdResult.Value);
        }

        [Fact]
        public async Task Create_ComArgumentException_DeveLancarArgumentException()
        {
            // Arrange
            var request = _createRequestFaker.Generate();
            var errorMessage = "Dados inválidos";

            _mockConsorcioService.Setup(s => s.CreateAsync(It.IsAny<Consorcio>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.Create(request, CancellationToken.None));
            Assert.Equal(errorMessage, exception.Message);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_ComIdValido_DeveRetornarOk()
        {
            // Arrange
            var consorcio = _consorcioFaker.Generate();
            var id = consorcio.Id;

            _mockConsorcioService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(consorcio);

            // Act
            var result = await _controller.GetById(id, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_ComConsorcioNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var id = 999;

            _mockConsorcioService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Consorcio?)null);

            // Act
            var result = await _controller.GetById(id, CancellationToken.None);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("Consórcio não encontrado", notFoundResult.Value?.ToString());
        }

        #endregion

        #region GetWithCotas Tests

        [Fact]
        public async Task GetWithCotas_ComIdValido_DeveRetornarOk()
        {
            // Arrange
            var consorcio = _consorcioFaker.Generate();
            var id = consorcio.Id;

            _mockConsorcioService.Setup(s => s.GetWithCotasAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(consorcio);

            // Act
            var result = await _controller.GetWithCotas(id, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetWithCotas_ComConsorcioNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var id = 999;

            _mockConsorcioService.Setup(s => s.GetWithCotasAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Consorcio?)null);

            // Act
            var result = await _controller.GetWithCotas(id, CancellationToken.None);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("Consórcio não encontrado", notFoundResult.Value?.ToString());
        }

        #endregion

        #region GetActive Tests

        [Fact]
        public async Task GetActive_DeveRetornarOkComListaDeConsorcios()
        {
            // Arrange
            var consorcios = _consorcioFaker.Generate(3);

            _mockConsorcioService.Setup(s => s.GetActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(consorcios);

            // Act
            var result = await _controller.GetActive(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        #endregion

        #region GetAvailableCotas Tests

        [Fact]
        public async Task GetAvailableCotas_ComIdValido_DeveRetornarOk()
        {
            // Arrange
            var id = 1;
            var cotas = new List<Cotas>(); // Lista vazia para simplificar

            _mockConsorcioService.Setup(s => s.GetAvailableCotasAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cotas);

            // Act
            var result = await _controller.GetAvailableCotas(id, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAvailableCotas_ComArgumentException_DeveLancarArgumentException()
        {
            // Arrange
            var id = 1;
            var errorMessage = "Consórcio não encontrado";

            _mockConsorcioService.Setup(s => s.GetAvailableCotasAsync(id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.GetAvailableCotas(id, CancellationToken.None));
            Assert.Equal(errorMessage, exception.Message);
        }

        #endregion

        #region AddCotas Tests

        [Fact]
        public async Task AddCotas_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var id = 1;
            var request = _addCotasRequestFaker.Generate();
            var consorcio = _consorcioFaker.Generate();
            var cotasCriadas = new List<Cotas>(); // Lista vazia para simplificar

            _mockConsorcioService.Setup(s => s.GetWithCotasAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(consorcio);
            _mockConsorcioService.Setup(s => s.AddCotasAsync(id, request.Quantidade, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cotasCriadas);

            // Act
            var result = await _controller.AddCotas(id, request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task AddCotas_ComQuantidadeInvalida_DeveRetornarBadRequest()
        {
            // Arrange
            var id = 1;
            var request = new AddCotasRequestDto { Quantidade = 0 };

            // Act
            var result = await _controller.AddCotas(id, request, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Quantidade deve ser maior que zero", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task AddCotas_ComConsorcioNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var id = 999;
            var request = _addCotasRequestFaker.Generate();

            _mockConsorcioService.Setup(s => s.GetWithCotasAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Consorcio?)null);

            // Act
            var result = await _controller.AddCotas(id, request, CancellationToken.None);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Consórcio não encontrado", notFoundResult.Value?.ToString());
        }

        #endregion

        #region AssignCotaToUsuario Tests

        [Fact]
        public async Task AssignCotaToUsuario_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var request = _assignCotaRequestFaker.Generate();

            _mockConsorcioService.Setup(s => s.AssignCotaToUsuarioAsync(request.CotaId, request.UsuarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AssignCotaToUser(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Cota atribuída com sucesso", okResult.Value?.ToString());
        }

        [Fact]
        public async Task AssignCotaToUsuario_ComFalha_DeveRetornarBadRequest()
        {
            // Arrange
            var request = _assignCotaRequestFaker.Generate();

            _mockConsorcioService.Setup(s => s.AssignCotaToUsuarioAsync(request.CotaId, request.UsuarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.AssignCotaToUser(request, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Não foi possível atribuir a cota", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task AssignCotaToUsuario_ComArgumentException_DeveLancarArgumentException()
        {
            // Arrange
            var request = _assignCotaRequestFaker.Generate();
            var errorMessage = "Dados inválidos";

            _mockConsorcioService.Setup(s => s.AssignCotaToUsuarioAsync(request.CotaId, request.UsuarioId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.AssignCotaToUser(request, CancellationToken.None));
            Assert.Equal(errorMessage, exception.Message);
        }

        [Fact]
        public async Task AssignCotaToUsuario_ComInvalidOperationException_DeveLancarInvalidOperationException()
        {
            // Arrange
            var request = _assignCotaRequestFaker.Generate();
            var errorMessage = "Cota já está atribuída";

            _mockConsorcioService.Setup(s => s.AssignCotaToUsuarioAsync(request.CotaId, request.UsuarioId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(errorMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.AssignCotaToUser(request, CancellationToken.None));
            Assert.Equal(errorMessage, exception.Message);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var consorcio = _consorcioFaker.Generate();
            var id = consorcio.Id;
            var request = _updateRequestFaker.Generate();

            _mockConsorcioService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(consorcio);
            _mockConsorcioService.Setup(s => s.UpdateAsync(It.IsAny<Consorcio>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(consorcio);

            // Act
            var result = await _controller.Update(id, request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Update_ComConsorcioNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var id = 999;
            var request = _updateRequestFaker.Generate();

            _mockConsorcioService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Consorcio?)null);

            // Act
            var result = await _controller.Update(id, request, CancellationToken.None);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("Consórcio não encontrado", notFoundResult.Value?.ToString());
        }

        [Fact]
        public async Task Update_ComArgumentException_DeveLancarArgumentException()
        {
            // Arrange
            var consorcio = _consorcioFaker.Generate();
            var id = consorcio.Id;
            var request = _updateRequestFaker.Generate();
            var errorMessage = "Dados inválidos";

            _mockConsorcioService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(consorcio);
            _mockConsorcioService.Setup(s => s.UpdateAsync(It.IsAny<Consorcio>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.Update(id, request, CancellationToken.None));
            Assert.Equal(errorMessage, exception.Message);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ComIdValido_DeveRetornarNoContent()
        {
            // Arrange
            var id = 1;

            _mockConsorcioService.Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id, CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ComArgumentException_DeveLancarArgumentException()
        {
            // Arrange
            var id = 999;
            var errorMessage = "Consórcio não encontrado";

            _mockConsorcioService.Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.Delete(id, CancellationToken.None));
            Assert.Equal(errorMessage, exception.Message);
        }

        #endregion
    }
}
