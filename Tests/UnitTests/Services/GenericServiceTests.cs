using Bogus;
using Moq;
using rian_p01_back.src.Data.Repositories.Interfaces;
using rian_p01_back.src.Services.Implementations;
using System.Linq.Expressions;
using rian_p01_back.Tests.Models;

namespace UnitTests.Services
{
    public class GenericServiceTests
    {
        private readonly Mock<IGenericRepository<TestEntity>> _mockRepository;
        private readonly GenericService<TestEntity> _service;
        private readonly Faker<TestEntity> _faker;

        public GenericServiceTests()
        {
            _mockRepository = new Mock<IGenericRepository<TestEntity>>();
            _service = new GenericService<TestEntity>(_mockRepository.Object);
            _faker = new Faker<TestEntity>()
                .RuleFor(d => d.Id, f => f.Random.Int(1, 1000))
                .RuleFor(d => d.Name, f => f.Name.FullName())
                .RuleFor(d => d.Email, f => f.Internet.Email())
                .RuleFor(d => d.CreatedAt, f => f.Date.Past())
                .RuleFor(d => d.IsActive, f => f.Random.Bool());
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoEntidadeExiste_DeveRetornarEntidade()
        {
            // Arrange
            var entity = _faker.Generate();
            _mockRepository.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            // Act
            var result = await _service.GetByIdAsync(entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity, result);
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoEntidadeNaoExiste_DeveRetornarNulo()
        {
            // Arrange
            var entityId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(entityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TestEntity)null!);

            // Act
            var result = await _service.GetByIdAsync(entityId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ObterTodosAsync_QuandoEntidadesExistem_DeveRetornarTodasEntidades()
        {
            // Arrange
            var entities = _faker.Generate(3);
            _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(entities);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(entities, result);
        }

        [Fact]
        public async Task ObterTodosAsync_QuandoNenhumaEntidadeExiste_DeveRetornarColecaoVazia()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TestEntity>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task BuscarAsync_ComPredicadoValido_DeveRetornarEntidadesCorrespondentes()
        {
            // Arrange
            var entities = _faker.Generate(5);
            _mockRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<TestEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(entities);

            // Act
            var result = await _service.FindAsync(e => e.Id > 0);

            // Assert
            Assert.Equal(entities, result);
        }

        [Fact]
        public async Task CriarAsync_ComEntidadeValida_DeveCriarERetornarEntidade()
        {
            // Arrange
            var entity = _faker.Generate();
            _mockRepository.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(entity);

            // Assert
            Assert.Equal(entity, result);
            _mockRepository.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_ComEntidadeNula_DeveLancarArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
        }

        [Fact]
        public async Task AtualizarAsync_ComEntidadeValida_DeveAtualizarERetornarEntidade()
        {
            // Arrange
            var entity = _faker.Generate();
            _mockRepository.Setup(r => r.Update(entity));
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(entity);

            // Assert
            Assert.Equal(entity, result);
            _mockRepository.Verify(r => r.Update(entity), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_ComEntidadeNula_DeveLancarArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(null!));
        }

        [Fact]
        public async Task DeletarAsync_PorId_QuandoEntidadeExiste_DeveRemoverEntidade()
        {
            // Arrange
            var entity = _faker.Generate();
            _mockRepository.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);
            _mockRepository.Setup(r => r.Remove(entity));
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _service.DeleteAsync(entity.Id);

            // Assert
            _mockRepository.Verify(r => r.Remove(entity), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeletarAsync_PorId_QuandoEntidadeNaoExiste_DeveLancarArgumentException()
        {
            // Arrange
            var entityId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(entityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TestEntity)null!);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteAsync(entityId));
        }

        [Fact]
        public async Task DeletarAsync_PorEntidade_QuandoEntidadeEValida_DeveRemoverEntidade()
        {
            // Arrange
            var entity = _faker.Generate();
            _mockRepository.Setup(r => r.Remove(entity));
            _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _service.DeleteAsync(entity);

            // Assert
            _mockRepository.Verify(r => r.Remove(entity), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeletarAsync_PorEntidade_QuandoEntidadeNula_DeveLancarArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.DeleteAsync(null!));
        }
    }
}
