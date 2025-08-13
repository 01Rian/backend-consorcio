using rian_p01_back.src.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace tests.UnitTests.Models
{
    public class ConsorcioTests
    {
        [Fact]
        public void Consorcio_DeveTerPropriedadesCorretas()
        {
            // Arrange & Act
            var consorcio = new Consorcio();

            // Assert
            Assert.Equal(0, consorcio.Id);
            Assert.Equal(string.Empty, consorcio.Nome);
            Assert.Equal(string.Empty, consorcio.Codigo);
            Assert.Equal(0, consorcio.ValorBem);
            Assert.Equal(0, consorcio.QuantidadeCotas);
            Assert.Equal(0, consorcio.PrazoMeses);
            Assert.Equal(0, consorcio.TaxaAdministracao);
            Assert.Equal(0, consorcio.FundoReserva);
            Assert.Equal(default(DateTime), consorcio.DataInicio);
            Assert.Equal(default(DateTime), consorcio.DataTermino);
            Assert.True(consorcio.Ativo);
            Assert.True(consorcio.DataCadastro <= DateTime.Now);
            Assert.Null(consorcio.DataAtualizacao);
            Assert.NotNull(consorcio.Cotas);
            Assert.Empty(consorcio.Cotas);
        }

        [Fact]
        public void Consorcio_DevePermitirDefinirPropriedades()
        {
            // Arrange
            var consorcio = new Consorcio();
            var dataInicio = DateTime.Now;
            var dataTermino = DateTime.Now.AddMonths(60);
            var dataAtualizacao = DateTime.Now;

            // Act
            consorcio.Id = 1;
            consorcio.Nome = "Consórcio Imóvel";
            consorcio.Codigo = "CONS001";
            consorcio.ValorBem = 500000.00m;
            consorcio.QuantidadeCotas = 100;
            consorcio.PrazoMeses = 60;
            consorcio.TaxaAdministracao = 15.50m;
            consorcio.FundoReserva = 2.00m;
            consorcio.DataInicio = dataInicio;
            consorcio.DataTermino = dataTermino;
            consorcio.Ativo = false;
            consorcio.DataAtualizacao = dataAtualizacao;

            // Assert
            Assert.Equal(1, consorcio.Id);
            Assert.Equal("Consórcio Imóvel", consorcio.Nome);
            Assert.Equal("CONS001", consorcio.Codigo);
            Assert.Equal(500000.00m, consorcio.ValorBem);
            Assert.Equal(100, consorcio.QuantidadeCotas);
            Assert.Equal(60, consorcio.PrazoMeses);
            Assert.Equal(15.50m, consorcio.TaxaAdministracao);
            Assert.Equal(2.00m, consorcio.FundoReserva);
            Assert.Equal(dataInicio, consorcio.DataInicio);
            Assert.Equal(dataTermino, consorcio.DataTermino);
            Assert.False(consorcio.Ativo);
            Assert.Equal(dataAtualizacao, consorcio.DataAtualizacao);
        }

        [Theory]
        [InlineData("", "CONS001", "Nome")]
        [InlineData("Teste", "", "Codigo")]
        [InlineData(null, "CONS001", "Nome")]
        [InlineData("Teste", null, "Codigo")]
        [InlineData("   ", "CONS001", "Nome")]
        public void Consorcio_CamposObrigatorios_NaoDevemSerVazios(string? nome, string? codigo, string campoInvalido)
        {
            // Arrange
            var consorcio = new Consorcio
            {
                Nome = nome ?? string.Empty,
                Codigo = codigo ?? string.Empty,
                ValorBem = 100000,
                QuantidadeCotas = 50,
                PrazoMeses = 60,
                TaxaAdministracao = 15,
                FundoReserva = 2
            };

            var context = new ValidationContext(consorcio);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(consorcio, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(campoInvalido));
        }

        [Theory]
        [InlineData(101, "Nome")]
        [InlineData(51, "Codigo")]
        public void Consorcio_CamposComTamanhoMaximo_DevemFalharQuandoExcedidos(int tamanho, string campo)
        {
            // Arrange
            var textoGrande = new string('A', tamanho);
            var consorcio = new Consorcio
            {
                Nome = campo == "Nome" ? textoGrande : "Teste",
                Codigo = campo == "Codigo" ? textoGrande : "CONS001",
                ValorBem = 100000,
                QuantidadeCotas = 50,
                PrazoMeses = 60,
                TaxaAdministracao = 15,
                FundoReserva = 2
            };

            var context = new ValidationContext(consorcio);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(consorcio, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(campo));
        }

    }
}
