using Bogus;
using Microsoft.EntityFrameworkCore;
using rian_p01_back.Tests.Models;

namespace rian_p01_back.Tests.Repositories
{
    public class GenericRepositoryTests : RepositoryTestBase
    {
        private readonly Faker<TestEntity> _entityFaker;

        public GenericRepositoryTests()
        {
            _entityFaker = new Faker<TestEntity>()
                .RuleFor(x => x.Name, f => f.Person.FullName)
                .RuleFor(x => x.Email, f => f.Person.Email)
                .RuleFor(x => x.CreatedAt, f => f.Date.Recent())
                .RuleFor(x => x.IsActive, f => f.Random.Bool());
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoEntidadeExiste_DeveRetornarEntidade()
        {
            // Arrange
            var entity = _entityFaker.Generate();
            await Context.Set<TestEntity>().AddAsync(entity);
            await Context.SaveChangesAsync();

            // Act
            var result = await Repository.GetByIdAsync(entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result!.Id);
            Assert.Equal(entity.Name, result.Name);
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoEntidadeNaoExiste_DeveRetornarNulo()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var result = await Repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ObterTodosAsync_QuandoEntidadesExistem_DeveRetornarTodasEntidades()
        {
            // Arrange
            var entities = _entityFaker.Generate(5);
            await Context.Set<TestEntity>().AddRangeAsync(entities);
            await Context.SaveChangesAsync();

            // Act
            var result = await Repository.GetAllAsync();

            // Assert
            Assert.Equal(5, result.Count());
            foreach (var entity in entities)
            {
                Assert.Contains(result, r => r.Id == entity.Id && r.Name == entity.Name);
            }
        }

        [Fact]
        public async Task ObterTodosAsync_QuandoNenhumaEntidadeExiste_DeveRetornarColecaoVazia()
        {
            // Act
            var result = await Repository.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task BuscarAsync_ComPredicadoValido_DeveRetornarEntidadesCorrespondentes()
        {
            // Arrange
            var activeEntities = _entityFaker.Generate(3);
            activeEntities.ForEach(e => e.IsActive = true);
            
            var inactiveEntities = _entityFaker.Generate(2);
            inactiveEntities.ForEach(e => e.IsActive = false);

            await Context.Set<TestEntity>().AddRangeAsync(activeEntities.Concat(inactiveEntities));
            await Context.SaveChangesAsync();

            // Act
            var result = await Repository.FindAsync(x => x.IsActive);

            // Assert
            Assert.Equal(3, result.Count());
            Assert.All(result, x => Assert.True(x.IsActive));
        }

        [Fact]
        public async Task AdicionarAsync_ComEntidadeValida_DeveAdicionarEntidadeAoContexto()
        {
            // Arrange
            var entity = _entityFaker.Generate();

            // Act
            var result = await Repository.AddAsync(entity);
            await Repository.SaveChangesAsync();

            // Assert
            Assert.Equal(entity, result);

            var savedEntity = await Context.Set<TestEntity>().FindAsync(entity.Id);
            Assert.NotNull(savedEntity);
            Assert.Equal(entity.Id, savedEntity.Id);
            Assert.Equal(entity.Name, savedEntity.Name);
            Assert.Equal(entity.Email, savedEntity.Email);
            Assert.Equal(entity.CreatedAt, savedEntity.CreatedAt);
            Assert.Equal(entity.IsActive, savedEntity.IsActive);
        }

        [Fact]
        public async Task AdicionarVariasAsync_ComEntidadesValidas_DeveAdicionarTodasAoContexto()
        {
            // Arrange
            var entities = _entityFaker.Generate(3);

            // Act
            await Repository.AddRangeAsync(entities);
            await Repository.SaveChangesAsync();

            // Assert
            var savedEntities = await Context.Set<TestEntity>().ToListAsync();
            Assert.Equal(3, savedEntities.Count);
            foreach (var entity in entities)
            {
                Assert.Contains(savedEntities, r => r.Id == entity.Id && r.Name == entity.Name);
            }
        }

        [Fact]
        public async Task AtualizarAsync_ComEntidadeValida_DeveAtualizarEntidade()
        {
            // Arrange
            var entity = _entityFaker.Generate();
            await Context.Set<TestEntity>().AddAsync(entity);
            await Context.SaveChangesAsync();

            var updatedName = "Updated Name";
            entity.Name = updatedName;

            // Act
            Repository.Update(entity);
            await Repository.SaveChangesAsync();

            // Assert
            var updatedEntity = await Context.Set<TestEntity>().FindAsync(entity.Id);
            Assert.NotNull(updatedEntity);
            Assert.Equal(updatedName, updatedEntity!.Name);
        }

        [Fact]
        public async Task RemoverAsync_ComEntidadeValida_DeveRemoverEntidade()
        {
            // Arrange
            var entity = _entityFaker.Generate();
            await Context.Set<TestEntity>().AddAsync(entity);
            await Context.SaveChangesAsync();

            // Act
            Repository.Remove(entity);
            await Repository.SaveChangesAsync();

            // Assert
            var removedEntity = await Context.Set<TestEntity>().FindAsync(entity.Id);
            Assert.Null(removedEntity);
        }

        [Fact]
        public async Task RemoverVariasAsync_ComEntidadesValidas_DeveRemoverTodasEntidades()
        {
            // Arrange
            var entities = _entityFaker.Generate(3);
            await Context.Set<TestEntity>().AddRangeAsync(entities);
            await Context.SaveChangesAsync();

            // Act
            Repository.RemoveRange(entities);
            await Repository.SaveChangesAsync();

            // Assert
            var remainingEntities = await Context.Set<TestEntity>().ToListAsync();
            Assert.Empty(remainingEntities);
        }

        [Fact]
        public async Task SalvarAlteracoesAsync_DeveRetornarNumeroDeRegistrosAfetados()
        {
            // Arrange
            var entities = _entityFaker.Generate(2);
            await Repository.AddRangeAsync(entities);

            // Act
            var result = await Repository.SaveChangesAsync();

            // Assert
            Assert.Equal(2, result);
        }
    }
}