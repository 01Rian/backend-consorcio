using rian_p01_back.src.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace tests.UnitTests.Models
{
    public class AdministradoraTests
    {
        [Fact]
        public void Administradora_DeveTerPropriedadesCorretas()
        {
            // Arrange & Act
            var administradora = new Administradora();

            // Assert
            Assert.Equal(0, administradora.Id);
            Assert.Equal(string.Empty, administradora.Nome);
            Assert.Equal(string.Empty, administradora.CNPJ);
            Assert.Null(administradora.Telefone);
            Assert.Null(administradora.Email);
            Assert.Null(administradora.Endereco);
            Assert.True(administradora.Ativo);
            Assert.True(administradora.DataCadastro <= DateTime.Now);
            Assert.Null(administradora.DataAtualizacao);
            Assert.NotNull(administradora.Consorcios);
            Assert.Empty(administradora.Consorcios);
        }

        [Fact]
        public void Administradora_DevePermitirDefinirPropriedades()
        {
            // Arrange
            var administradora = new Administradora();
            var dataAtualizacao = DateTime.Now;
            var endereco = new Endereco
            {
                Logradouro = "Rua Teste, 123",
                Numero = "123",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                CEP = "01234-567"
            };

            // Act
            administradora.Id = 1;
            administradora.Nome = "Administradora Teste";
            administradora.CNPJ = "12.345.678/0001-90";
            administradora.Telefone = "(11) 1234-5678";
            administradora.Email = "contato@teste.com";
            administradora.Endereco = endereco;
            administradora.Ativo = false;
            administradora.DataAtualizacao = dataAtualizacao;

            // Assert
            Assert.Equal(1, administradora.Id);
            Assert.Equal("Administradora Teste", administradora.Nome);
            Assert.Equal("12.345.678/0001-90", administradora.CNPJ);
            Assert.Equal("(11) 1234-5678", administradora.Telefone);
            Assert.Equal("contato@teste.com", administradora.Email);
            Assert.NotNull(administradora.Endereco);
            Assert.Equal("Rua Teste, 123", administradora.Endereco.Logradouro);
            Assert.Equal("São Paulo", administradora.Endereco.Cidade);
            Assert.Equal("SP", administradora.Endereco.Estado);
            Assert.Equal("01234-567", administradora.Endereco.CEP);
            Assert.False(administradora.Ativo);
            Assert.Equal(dataAtualizacao, administradora.DataAtualizacao);
        }

        [Theory]
        [InlineData("", "12.345.678/0001-90", "Nome")]
        [InlineData("Teste", "", "CNPJ")]
        [InlineData(null, "12.345.678/0001-90", "Nome")]
        [InlineData("Teste", null, "CNPJ")]
        [InlineData("   ", "12.345.678/0001-90", "Nome")]
        public void Administradora_CamposObrigatorios_NaoDevemSerVazios(string? nome, string? cnpj, string campoInvalido)
        {
            // Arrange
            var administradora = new Administradora
            {
                Nome = nome ?? string.Empty,
                CNPJ = cnpj ?? string.Empty
            };

            var context = new ValidationContext(administradora);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(administradora, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(campoInvalido));
        }

        [Theory]
        [InlineData(101, "Nome")]
        [InlineData(19, "CNPJ")]
        public void Administradora_CamposComTamanhoMaximo_DevemFalharQuandoExcedidos(int tamanho, string campo)
        {
            // Arrange
            var textoGrande = new string('A', tamanho);
            var administradora = new Administradora
            {
                Nome = campo == "Nome" ? textoGrande : "Teste",
                CNPJ = campo == "CNPJ" ? textoGrande : "12.345.678/0001-90"
            };

            var context = new ValidationContext(administradora);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(administradora, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(campo));
        }
    }
}
