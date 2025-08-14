using Bogus;
using Moq;
using rian_p01_back.src.Data.Repositories.Interfaces;
using rian_p01_back.src.Models.Entities;
using rian_p01_back.src.Services.Implementations;

namespace UnitTests.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _mockRepository;
        private readonly UsuarioService _service;
        private readonly Faker<Usuario> _faker;

        public UsuarioServiceTests()
        {
            _mockRepository = new Mock<IUsuarioRepository>();
            _service = new UsuarioService(_mockRepository.Object);
            _faker = new Faker<Usuario>()
                .RuleFor(u => u.Id, f => f.Random.Int(1, 1000))
                .RuleFor(u => u.Nome, f => f.Name.FullName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.CPF, f => f.Random.Replace("###########"))
                .RuleFor(u => u.DataCadastro, f => f.Date.Past())
                .RuleFor(u => u.DataAtualizacao, f => f.Date.Recent())
                .RuleFor(u => u.Senha, f => f.Internet.Password());
        }

        [Fact]
        public async Task GetByEmailAsync_ComEmailValido_DeveRetornarUsuario()
        {
            // Arrange
            var usuario = _faker.Generate();
            _mockRepository.Setup(r => r.GetByEmailAsync(usuario.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var result = await _service.GetByEmailAsync(usuario.Email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usuario.Email, result.Email);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetByEmailAsync_ComEmailInvalido_DeveLancarArgumentException(string email)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByEmailAsync(email));
        }

        [Fact]
        public async Task GetByEmailAsync_ComEmailNulo_DeveLancarArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByEmailAsync(null!));
        }

        [Fact]
        public async Task GetByCpfAsync_ComCpfValido_DeveRetornarUsuario()
        {
            // Arrange
            var usuario = _faker.Generate();
            _mockRepository.Setup(r => r.GetByCpfAsync(usuario.CPF, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Act
            var result = await _service.GetByCpfAsync(usuario.CPF);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usuario.CPF, result.CPF);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetByCpfAsync_ComCpfInvalido_DeveLancarArgumentException(string cpf)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByCpfAsync(cpf));
        }

        [Fact]
        public async Task GetByCpfAsync_ComCpfNulo_DeveLancarArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByCpfAsync(null!));
        }

        [Fact]
        public async Task CreateAsync_ComUsuarioValido_DeveCriarUsuario()
        {
            // Arrange
            var usuario = _faker.Generate();
            _mockRepository.Setup(r => r.GetByEmailAsync(usuario.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);
            _mockRepository.Setup(r => r.GetByCpfAsync(usuario.CPF, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);
            _mockRepository.Setup(r => r.AddAsync(usuario, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(usuario);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.DataCadastro > DateTime.MinValue);
        }

        [Fact]
        public async Task CreateAsync_ComEmailJaExistente_DeveLancarInvalidOperationException()
        {
            // Arrange
            var usuario = _faker.Generate();
            var usuarioExistente = _faker.Generate();
            _mockRepository.Setup(r => r.GetByEmailAsync(usuario.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioExistente);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(usuario));
            Assert.Contains("Usuário já cadastrado", exception.Message);
        }

        [Fact]
        public async Task CreateAsync_ComCpfJaExistente_DeveLancarInvalidOperationException()
        {
            // Arrange
            var usuario = _faker.Generate();
            var usuarioExistente = _faker.Generate();
            _mockRepository.Setup(r => r.GetByEmailAsync(usuario.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);
            _mockRepository.Setup(r => r.GetByCpfAsync(usuario.CPF, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioExistente);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(usuario));
            Assert.Contains("Usuário já cadastrado", exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_ComUsuarioValido_DeveAtualizarUsuario()
        {
            // Arrange
            var usuario = _faker.Generate();
            _mockRepository.Setup(r => r.GetByIdAsync(usuario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            _mockRepository.Setup(r => r.GetByEmailAsync(usuario.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);
            _mockRepository.Setup(r => r.GetByCpfAsync(usuario.CPF, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);
            _mockRepository.Setup(r => r.Update(usuario));
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(usuario);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.DataAtualizacao > DateTime.MinValue);
        }

        [Fact]
        public async Task UpdateAsync_ComEmailJaExistenteParaOutroUsuario_DeveLancarInvalidOperationException()
        {
            // Arrange
            var usuario = _faker.Generate();
            var usuarioExistente = _faker.Generate();
            usuarioExistente.Id = usuario.Id + 1; // ID diferente
            usuarioExistente.Email = usuario.Email; // Email igual
            _mockRepository.Setup(r => r.GetByIdAsync(usuario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            _mockRepository.Setup(r => r.GetByEmailAsync(usuario.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioExistente);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(usuario));
            Assert.Contains("Usuário já cadastrado", exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_ComCpfJaExistenteParaOutroUsuario_DeveLancarInvalidOperationException()
        {
            // Arrange
            var usuario = _faker.Generate();
            var usuarioExistente = _faker.Generate();
            usuarioExistente.Id = usuario.Id + 1; // ID diferente
            usuarioExistente.CPF = usuario.CPF; // CPF igual
            _mockRepository.Setup(r => r.GetByIdAsync(usuario.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
            _mockRepository.Setup(r => r.GetByEmailAsync(usuario.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);
            _mockRepository.Setup(r => r.GetByCpfAsync(usuario.CPF, It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioExistente);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(usuario));
            Assert.Contains("Usuário já cadastrado", exception.Message);
        }
    }
}
