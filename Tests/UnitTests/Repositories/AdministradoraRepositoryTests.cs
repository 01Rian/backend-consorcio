using Bogus;
using rian_p01_back.src.Data.Repositories.Implementations;
using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.Tests.Repositories
{
    public class AdministradoraRepositoryTests : RepositoryTestBase
    {
        private readonly Faker<Administradora> _faker;

        public AdministradoraRepositoryTests()
        {
            _faker = new Faker<Administradora>()
                .RuleFor(x => x.Nome, f => f.Company.CompanyName())
                .RuleFor(x => x.CNPJ, f => f.Random.Replace("##############"));
        }

        [Fact]
        public async Task GetByCnpjAsync_DeveRetornarAdministradoraCorreta()
        {
            // Arrange
            var administradora = _faker.Generate();
            await Context.Set<Administradora>().AddAsync(administradora);
            await Context.SaveChangesAsync();

            // Act
            var repo = new AdministradoraRepository(Context);
            var result = await repo.GetByCnpjAsync(administradora.CNPJ);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(administradora.CNPJ, result!.CNPJ);
        }

        [Fact]
        public async Task GetByCnpjAsync_DeveRetornarNulo_QuandoNaoExiste()
        {
            // Arrange
            var repo = new AdministradoraRepository(Context);

            // Act
            var result = await repo.GetByCnpjAsync("00000000000000");

            // Assert
            Assert.Null(result);
        }
    }
}
