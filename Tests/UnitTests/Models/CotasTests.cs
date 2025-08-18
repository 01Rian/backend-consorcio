using rian_p01_back.src.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace tests.UnitTests.Models
{
    public class CotasTests
    {
        [Fact]
        public void Cotas_DeveTerPropriedadesCorretas()
        {
            // Arrange & Act
            var cota = new Cotas();

            // Assert
            Assert.Equal(0, cota.Id);
            Assert.Equal(string.Empty, cota.NumeroCota);
            Assert.Equal(0, cota.ValorParcela);
            Assert.Equal(0, cota.ValorPago);
            Assert.Equal(0, cota.ParcelasPagas);
            Assert.False(cota.Contemplada);
            Assert.Null(cota.DataContemplacao);
            Assert.Equal(StatusCota.Ativo, cota.Status);
            Assert.True(cota.Ativo);
            Assert.True(cota.DataCadastro <= DateTime.Now);
            Assert.Null(cota.DataAtualizacao);
            Assert.Equal(0, cota.ConsorcioId);
            Assert.Null(cota.UsuarioId);
        }

        [Fact]
        public void Cotas_DevePermitirDefinirPropriedades()
        {
            // Arrange
            var cota = new Cotas();
            var dataContemplacao = DateTime.Now;
            var dataAtualizacao = DateTime.Now;

            // Act
            cota.Id = 1;
            cota.NumeroCota = "001";
            cota.ValorParcela = 5000.00m;
            cota.ValorPago = 25000.00m;
            cota.ParcelasPagas = 5;
            cota.Contemplada = true;
            cota.DataContemplacao = dataContemplacao;
            cota.Status = StatusCota.Contemplado;
            cota.Ativo = false;
            cota.DataAtualizacao = dataAtualizacao;
            cota.ConsorcioId = 1;
            cota.UsuarioId = 1;

            // Assert
            Assert.Equal(1, cota.Id);
            Assert.Equal("001", cota.NumeroCota);
            Assert.Equal(5000.00m, cota.ValorParcela);
            Assert.Equal(25000.00m, cota.ValorPago);
            Assert.Equal(5, cota.ParcelasPagas);
            Assert.True(cota.Contemplada);
            Assert.Equal(dataContemplacao, cota.DataContemplacao);
            Assert.Equal(StatusCota.Contemplado, cota.Status);
            Assert.False(cota.Ativo);
            Assert.Equal(dataAtualizacao, cota.DataAtualizacao);
            Assert.Equal(1, cota.ConsorcioId);
            Assert.Equal(1, cota.UsuarioId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Cotas_NumeroCota_DeveSerObrigatorio(string? numeroCota)
        {
            // Arrange
            var cota = new Cotas
            {
                NumeroCota = numeroCota ?? string.Empty,
                ValorParcela = 1000,
                ConsorcioId = 1
            };

            var context = new ValidationContext(cota);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(cota, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("NumeroCota"));
        }

        [Theory]
        [InlineData(0, "ValorParcela")]
        [InlineData(-1, "ValorParcela")]
        public void Cotas_ValorParcela_DeveSerMaiorQueZero(decimal valor, string campo)
        {
            // Arrange
            var cota = new Cotas
            {
                NumeroCota = "001",
                ValorParcela = valor,
                ConsorcioId = 1
            };

            var context = new ValidationContext(cota);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(cota, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(campo));
        }

        [Theory]
        [InlineData(0, "ConsorcioId")]
        [InlineData(-1, "ConsorcioId")]
        public void Cotas_ConsorcioId_DeveSerMaiorQueZero(int valor, string campo)
        {
            // Arrange
            var cota = new Cotas
            {
                NumeroCota = "001",
                ValorParcela = 1000,
                ConsorcioId = valor
            };

            var context = new ValidationContext(cota);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(cota, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(campo));
        }

        [Fact]
        public void Cotas_Status_DeveSerObrigatorio()
        {
            // Arrange
            var cota = new Cotas
            {
                NumeroCota = "001",
                ValorParcela = 1000,
                ConsorcioId = 1
            };

            var context = new ValidationContext(cota);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(cota, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Equal(StatusCota.Ativo, cota.Status);
        }

        [Fact]
        public void Cotas_NumeroCota_DeveTerTamanhoMaximo20Caracteres()
        {
            // Arrange
            var numeroGrande = new string('1', 21);
            var cota = new Cotas
            {
                NumeroCota = numeroGrande,
                ValorParcela = 1000,
                ConsorcioId = 1
            };

            var context = new ValidationContext(cota);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(cota, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("NumeroCota"));
        }

        [Theory]
        [InlineData(StatusCota.Ativo)]
        [InlineData(StatusCota.Contemplado)]
        [InlineData(StatusCota.Suspenso)]
        [InlineData(StatusCota.Quitado)]
        public void Cotas_Status_DeveAceitarTodosOsValoresDoEnum(StatusCota status)
        {
            // Arrange & Act
            var cota = new Cotas
            {
                NumeroCota = "001",
                ValorParcela = 1000,
                ConsorcioId = 1,
                Status = status
            };

            // Assert
            Assert.Equal(status, cota.Status);
        }

        [Fact]
        public void Cotas_UsuarioId_PodeSerNulo()
        {
            // Arrange & Act
            var cota = new Cotas
            {
                NumeroCota = "001",
                ValorParcela = 1000,
                ConsorcioId = 1,
                UsuarioId = null
            };

            var context = new ValidationContext(cota);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(cota, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Null(cota.UsuarioId);
        }
    }
}
