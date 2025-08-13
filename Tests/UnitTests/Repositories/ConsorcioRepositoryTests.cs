using Bogus;
using rian_p01_back.src.Data.Repositories.Implementations;
using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.Tests.Repositories
{
    public class ConsorcioRepositoryTests : RepositoryTestBase
    {
        private readonly Faker<Consorcio> _faker;

        public ConsorcioRepositoryTests()
        {
            _faker = new Faker<Consorcio>()
                .RuleFor(x => x.Nome, f => f.Commerce.ProductName())
                .RuleFor(x => x.Codigo, f => f.Random.AlphaNumeric(8));
        }

        [Fact]
        public async Task GetWithCotasAsync_DeveRetornarConsorcioComCotas()
        {
            // Arrange
            var consorcio = _faker.Generate();
            consorcio.Cotas = new List<Cotas> { new Cotas { NumeroCota = "001" } };
            await Context.Set<Consorcio>().AddAsync(consorcio);
            await Context.SaveChangesAsync();

            // Act
            var repo = new ConsorcioRepository(Context);
            var result = await repo.GetWithCotasAsync(consorcio.Id);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result!.Cotas);
            Assert.Single(result.Cotas);
        }
    }
}
