using Bogus;
using rian_p01_back.src.Data.Repositories.Implementations;
using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.Tests.Repositories
{
    public class CotasRepositoryTests : RepositoryTestBase
    {
        private readonly Faker<Cotas> _faker;

        public CotasRepositoryTests()
        {
            _faker = new Faker<Cotas>()
                .RuleFor(x => x.NumeroCota, f => f.Random.AlphaNumeric(5));
        }

        [Fact]
        public async Task GetByConsorcioAsync_DeveRetornarCotasCorretas()
        {
            // Arrange
            var consorcio = new Consorcio { Nome = "Consorcio Teste" };
            var cota = _faker.Generate();
            cota.Consorcio = consorcio;

            await Context.Set<Consorcio>().AddAsync(consorcio);
            await Context.Set<Cotas>().AddAsync(cota);
            await Context.SaveChangesAsync();

            // Act
            var repo = new CotasRepository(Context);
            var result = await repo.GetByConsorcioAsync(consorcio.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.All(result, c => Assert.Equal(consorcio.Id, c.ConsorcioId));
        }

        [Fact]
        public async Task GetByUsuarioAsync_DeveRetornarCotasCorretas()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.UsuarioId = 456;
            await Context.Set<Cotas>().AddAsync(cota);
            await Context.SaveChangesAsync();

            // Act
            var repo = new CotasRepository(Context);
            var result = await repo.GetByUsuarioAsync(456);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.All(result, c => Assert.Equal(456, c.UsuarioId));
        }

        [Fact]
        public async Task GetDetalhadaAsync_DeveRetornarCotaComConsorcioEUsuario()
        {
            // Arrange
            var cota = _faker.Generate();
            cota.Consorcio = new Consorcio { Nome = "Teste" };
            cota.Usuario = new Usuario { Nome = "Usuário" };
            await Context.Set<Cotas>().AddAsync(cota);
            await Context.SaveChangesAsync();

            // Act
            var repo = new CotasRepository(Context);
            var result = await repo.GetDetalhadaAsync(cota.Id);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result!.Consorcio);
            Assert.NotNull(result.Usuario);
        }
    }
}
