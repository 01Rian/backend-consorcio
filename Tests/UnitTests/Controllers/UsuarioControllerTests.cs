using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
using rian_p01_back.src.Models.Entities;
using rian_p01_back.src.Services.Interfaces;
using rian_p01_back.Src.Controllers.Usuario;

namespace UnitTests.Controllers
{
    public class UsuarioControllerTests
    {
        private readonly Mock<IUsuarioService> _mockUsuarioService;
        private readonly UsuarioController _controller;
        private readonly Faker<Usuario> _usuarioFaker;
        private readonly Faker<CreateUsuarioRequestDto> _createRequestFaker;
        private readonly Faker<LoginUsuarioRequestDto> _loginRequestFaker;
        private readonly Faker<UpdateUsuarioRequestDto> _updateRequestFaker;

        public UsuarioControllerTests()
        {
            // Configurar variáveis de ambiente necessárias para JWT
            Environment.SetEnvironmentVariable("JWT_SECRET", "my-super-secret-key-for-testing-purposes-only-12345678901234567890");
            Environment.SetEnvironmentVariable("JWT_ISSUER", "test-issuer");
            Environment.SetEnvironmentVariable("JWT_AUDIENCE", "test-audience");
            Environment.SetEnvironmentVariable("JWT_EXPIRY_MINUTES", "60");

            _mockUsuarioService = new Mock<IUsuarioService>();
            _controller = new UsuarioController(_mockUsuarioService.Object);

            _usuarioFaker = new Faker<Usuario>()
                .RuleFor(u => u.Id, f => f.Random.Int(1, 1000))
                .RuleFor(u => u.Nome, f => f.Name.FullName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.CPF, f => f.Random.Replace("###########"))
                .RuleFor(u => u.Telefone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Celular, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.DataNascimento, f => f.Date.Past(30))
                .RuleFor(u => u.DataCadastro, f => f.Date.Past())
                .RuleFor(u => u.DataAtualizacao, f => f.Date.Recent())
                .RuleFor(u => u.Senha, f => f.Internet.Password())
                .RuleFor(u => u.Ativo, f => true);

            _createRequestFaker = new Faker<CreateUsuarioRequestDto>()
                .RuleFor(r => r.Nome, f => f.Name.FullName())
                .RuleFor(r => r.Email, f => f.Internet.Email())
                .RuleFor(r => r.Senha, f => f.Internet.Password(6))
                .RuleFor(r => r.CPF, f => f.Random.Replace("###########"))
                .RuleFor(r => r.Telefone, f => f.Phone.PhoneNumber())
                .RuleFor(r => r.Celular, f => f.Phone.PhoneNumber())
                .RuleFor(r => r.DataNascimento, f => f.Date.Past(30));

            _loginRequestFaker = new Faker<LoginUsuarioRequestDto>()
                .RuleFor(r => r.Email, f => f.Internet.Email())
                .RuleFor(r => r.Senha, f => f.Internet.Password(6));

            _updateRequestFaker = new Faker<UpdateUsuarioRequestDto>()
                .RuleFor(r => r.Nome, f => f.Name.FullName())
                .RuleFor(r => r.Telefone, f => f.Phone.PhoneNumber())
                .RuleFor(r => r.Celular, f => f.Phone.PhoneNumber())
                .RuleFor(r => r.DataNascimento, f => f.Date.Past(30));
        }

        #region Create Tests

        [Fact]
        public async Task Create_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var request = _createRequestFaker.Generate();
            var usuario = _usuarioFaker.Generate();

            _mockUsuarioService.Setup(s => s.CreateAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            Assert.NotNull(createdResult.Value);
        }

        [Fact]
        public async Task Create_ComInvalidOperationException_DeveLancarInvalidOperationException()
        {
            // Arrange
            var request = _createRequestFaker.Generate();
            var errorMessage = "Email já existe";

            _mockUsuarioService.Setup(s => s.CreateAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(errorMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Create(request));
            Assert.Equal(errorMessage, exception.Message);
        }

        [Fact]
        public async Task Create_ComArgumentException_DeveLancarArgumentException()
        {
            // Arrange
            var request = _createRequestFaker.Generate();
            var errorMessage = "Dados inválidos";

            _mockUsuarioService.Setup(s => s.CreateAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.Create(request));
            Assert.Equal(errorMessage, exception.Message);
        }

        #endregion

        #region Login Tests

        [Fact]
        public async Task Login_ComCredenciaisValidas_DeveRetornarOk()
        {
            // Arrange
            var request = _loginRequestFaker.Generate();
            var usuario = _usuarioFaker.Generate();
            usuario.Email = request.Email;

            _mockUsuarioService.Setup(s => s.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            _mockUsuarioService.Setup(s => s.ValidatePassword(usuario, request.Senha))
                .Returns(true);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Login_ComUsuarioNaoEncontrado_DeveRetornarUnauthorized()
        {
            // Arrange
            var request = _loginRequestFaker.Generate();

            _mockUsuarioService.Setup(s => s.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Contains("Email ou senha inválidos", unauthorizedResult.Value?.ToString());
        }

        [Fact]
        public async Task Login_ComSenhaInvalida_DeveRetornarUnauthorized()
        {
            // Arrange
            var request = _loginRequestFaker.Generate();
            var usuario = _usuarioFaker.Generate();

            _mockUsuarioService.Setup(s => s.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            _mockUsuarioService.Setup(s => s.ValidatePassword(usuario, request.Senha))
                .Returns(false);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Contains("Email ou senha inválidos", unauthorizedResult.Value?.ToString());
        }

        #endregion

        #region GetAll Tests

        [Fact]
        public async Task GetAll_DeveRetornarOkComListaDeUsuarios()
        {
            // Arrange
            var usuarios = _usuarioFaker.Generate(3);

            _mockUsuarioService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarios);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_ComIdValido_DeveRetornarOk()
        {
            // Arrange
            var usuario = _usuarioFaker.Generate();
            var id = usuario.Id;

            _mockUsuarioService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var result = await _controller.GetById(id);

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
            var result = await _controller.GetById(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("ID deve ser maior que zero", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task GetById_ComUsuarioNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var id = 999;

            _mockUsuarioService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("Usuário não encontrado", notFoundResult.Value?.ToString());
        }

        #endregion

        #region GetByEmail Tests

        [Fact]
        public async Task GetByEmail_ComEmailValido_DeveRetornarOk()
        {
            // Arrange
            var usuario = _usuarioFaker.Generate();
            var email = usuario.Email;

            _mockUsuarioService.Setup(s => s.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var result = await _controller.GetByEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetByEmail_ComEmailInvalido_DeveRetornarBadRequest(string email)
        {
            // Act
            var result = await _controller.GetByEmail(email);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Email é obrigatório", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task GetByEmail_ComUsuarioNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var email = "naoexiste@teste.com";

            _mockUsuarioService.Setup(s => s.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _controller.GetByEmail(email);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("Usuário não encontrado", notFoundResult.Value?.ToString());
        }

        [Fact]
        public async Task GetByEmail_ComEmailNull_DeveRetornarBadRequest()
        {
            // Act
            var result = await _controller.GetByEmail(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Email é obrigatório", badRequestResult.Value?.ToString());
        }

        #endregion

        #region GetByCpf Tests

        [Fact]
        public async Task GetByCpf_ComCpfValido_DeveRetornarOk()
        {
            // Arrange
            var usuario = _usuarioFaker.Generate();
            var cpf = usuario.CPF;

            _mockUsuarioService.Setup(s => s.GetByCpfAsync(cpf, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var result = await _controller.GetByCpf(cpf);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetByCpf_ComCpfInvalido_DeveRetornarBadRequest(string cpf)
        {
            // Act
            var result = await _controller.GetByCpf(cpf);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("CPF é obrigatório", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task GetByCpf_ComUsuarioNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var cpf = "12345678901";

            _mockUsuarioService.Setup(s => s.GetByCpfAsync(cpf, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _controller.GetByCpf(cpf);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("Usuário não encontrado", notFoundResult.Value?.ToString());
        }

        [Fact]
        public async Task GetByCpf_ComCpfNull_DeveRetornarBadRequest()
        {
            // Act
            var result = await _controller.GetByCpf(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("CPF é obrigatório", badRequestResult.Value?.ToString());
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var usuario = _usuarioFaker.Generate();
            var id = usuario.Id;
            var request = _updateRequestFaker.Generate();

            _mockUsuarioService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            _mockUsuarioService.Setup(s => s.UpdateAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Update_ComIdInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var id = 0;
            var request = _updateRequestFaker.Generate();

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("ID deve ser maior que zero", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task Update_ComUsuarioNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var id = 999;
            var request = _updateRequestFaker.Generate();

            _mockUsuarioService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("Usuário não encontrado", notFoundResult.Value?.ToString());
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ComIdValido_DeveRetornarNoContent()
        {
            // Arrange
            var usuario = _usuarioFaker.Generate();
            var id = usuario.Id;

            _mockUsuarioService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            _mockUsuarioService.Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ComIdInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var id = 0;

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("ID deve ser maior que zero", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task Delete_ComUsuarioNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var id = 999;

            _mockUsuarioService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Usuário não encontrado", notFoundResult.Value?.ToString());
        }

        #endregion
    }
}
