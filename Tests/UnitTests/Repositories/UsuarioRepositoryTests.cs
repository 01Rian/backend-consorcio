using Bogus;
using rian_p01_back.src.Data.Repositories.Implementations;
using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.Tests.Repositories
{
    public class UsuarioRepositoryTests : RepositoryTestBase
    {
        private readonly Faker<Usuario> _faker;

        public UsuarioRepositoryTests()
        {
            _faker = new Faker<Usuario>()
                .RuleFor(x => x.Nome, f => f.Person.FullName)
                .RuleFor(x => x.Email, f => f.Person.Email)
                .RuleFor(x => x.CPF, f => f.Random.Replace("###########"));
        }

        [Fact]
        public async Task GetByEmailAsync_DeveRetornarUsuarioCorreto()
        {
            // Arrange
            var usuario = _faker.Generate();
            await Context.Set<Usuario>().AddAsync(usuario);
            await Context.SaveChangesAsync();

            // Act
            var repo = new UsuarioRepository(Context);
            var result = await repo.GetByEmailAsync(usuario.Email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usuario.Email, result!.Email);
        }

        [Fact]
        public async Task GetByCpfAsync_DeveRetornarUsuarioCorreto()
        {
            // Arrange
            var usuario = _faker.Generate();
            await Context.Set<Usuario>().AddAsync(usuario);
            await Context.SaveChangesAsync();

            // Act
            var repo = new UsuarioRepository(Context);
            var result = await repo.GetByCpfAsync(usuario.CPF);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usuario.CPF, result!.CPF);
        }
    }
}
