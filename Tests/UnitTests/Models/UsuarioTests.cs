using rian_p01_back.src.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace tests.UnitTests.Models
{
    public class UsuarioTests
    {
        [Fact]
        public void Usuario_DeveTerPropriedadesCorretas()
        {
            // Arrange & Act
            var usuario = new Usuario();

            // Assert
            Assert.Equal(0, usuario.Id);
            Assert.Equal(string.Empty, usuario.Nome);
            Assert.Equal(string.Empty, usuario.Email);
            Assert.Equal(string.Empty, usuario.Senha);
            Assert.Equal(string.Empty, usuario.CPF);
            Assert.Null(usuario.Telefone);
            Assert.Null(usuario.Celular);
            Assert.Null(usuario.DataNascimento);
            Assert.Null(usuario.Endereco);
            Assert.True(usuario.Ativo);
            Assert.True(usuario.DataCadastro <= DateTime.Now);
            Assert.Null(usuario.DataAtualizacao);
            Assert.NotNull(usuario.Cotas);
            Assert.Empty(usuario.Cotas);
        }

        [Fact]
        public void Usuario_DevePermitirDefinirPropriedades()
        {
            // Arrange
            var usuario = new Usuario();
            var dataNascimento = new DateTime(1990, 1, 1);
            var dataAtualizacao = DateTime.Now;
            var endereco = new Endereco
            {
                Logradouro = "Rua das Flores, 123",
                Numero = "123",
                Complemento = "Apto 45",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                CEP = "01234-567"
            };

            // Act
            usuario.Id = 1;
            usuario.Nome = "João Silva";
            usuario.Email = "joao@teste.com";
            usuario.Senha = "senhaSegura123";
            usuario.CPF = "123.456.789-00";
            usuario.Telefone = "(11) 1234-5678";
            usuario.Celular = "(11) 98765-4321";
            usuario.DataNascimento = dataNascimento;
            usuario.Endereco = endereco;
            usuario.Ativo = false;
            usuario.DataAtualizacao = dataAtualizacao;

            // Assert
            Assert.Equal(1, usuario.Id);
            Assert.Equal("João Silva", usuario.Nome);
            Assert.Equal("joao@teste.com", usuario.Email);
            Assert.Equal("senhaSegura123", usuario.Senha);
            Assert.Equal("123.456.789-00", usuario.CPF);
            Assert.Equal("(11) 1234-5678", usuario.Telefone);
            Assert.Equal("(11) 98765-4321", usuario.Celular);
            Assert.Equal(dataNascimento, usuario.DataNascimento);
            Assert.Equal(endereco, usuario.Endereco);
            Assert.False(usuario.Ativo);
            Assert.Equal(dataAtualizacao, usuario.DataAtualizacao);
        }

        [Theory]
        [InlineData("", "test@test.com", "senha123", "123.456.789-00", "Nome")]
        [InlineData("Teste", "", "senha123", "123.456.789-00", "Email")]
        [InlineData("Teste", "test@test.com", "", "123.456.789-00", "Senha")]
        [InlineData("Teste", "test@test.com", "senha123", "", "CPF")]
        [InlineData(null, "test@test.com", "senha123", "123.456.789-00", "Nome")]
        [InlineData("Teste", null, "senha123", "123.456.789-00", "Email")]
        [InlineData("Teste", "test@test.com", null, "123.456.789-00", "Senha")]
        [InlineData("Teste", "test@test.com", "senha123", null, "CPF")]
        [InlineData("   ", "test@test.com", "senha123", "123.456.789-00", "Nome")]
        public void Usuario_CamposObrigatorios_NaoDevemSerVazios(string? nome, string? email, string? senha, string? cpf, string campoInvalido)
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = nome ?? string.Empty,
                Email = email ?? string.Empty,
                Senha = senha ?? string.Empty,
                CPF = cpf ?? string.Empty
            };

            var context = new ValidationContext(usuario);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(usuario, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(campoInvalido));
        }

        [Theory]
        [InlineData("email-invalido")]
        [InlineData("teste.com")]
        [InlineData("@dominio.com")]
        [InlineData("teste@")]
        [InlineData("teste@@dominio.com")]
        public void Usuario_Email_DeveTerFormatoValido(string email)
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Email = email,
                Senha = "senha123",
                CPF = "123.456.789-00"
            };

            var context = new ValidationContext(usuario);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(usuario, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Theory]
        [InlineData("teste@dominio.com")]
        [InlineData("usuario.teste@empresa.com.br")]
        [InlineData("email+tag@site.co.uk")]
        [InlineData("user123@test-domain.org")]
        public void Usuario_Email_DeveTerFormatoValido_CasosValidos(string email)
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Email = email,
                Senha = "senha123",
                CPF = "123.456.789-00"
            };

            var context = new ValidationContext(usuario);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(usuario, context, results, true);

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData(101, "Nome")]
        [InlineData(15, "CPF")]
        [InlineData(256, "Senha")]
        public void Usuario_CamposComTamanhoMaximo_DevemFalharQuandoExcedidos(int tamanho, string campo)
        {
            // Arrange
            var textoGrande = new string('A', tamanho);
            var usuario = new Usuario
            {
                Nome = campo == "Nome" ? textoGrande : "Teste",
                Email = "test@test.com",
                Senha = campo == "Senha" ? textoGrande : "senha123",
                CPF = campo == "CPF" ? textoGrande : "123.456.789-00"
            };

            var context = new ValidationContext(usuario);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(usuario, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(campo));
        }

        [Fact]
        public void Usuario_Endereco_DevePermitirValoresNulos()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Email = "test@test.com",
                Senha = "senha123",
                CPF = "123.456.789-00",
                Endereco = null
            };

            var context = new ValidationContext(usuario);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(usuario, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Null(usuario.Endereco);
        }

        [Fact]
        public void Usuario_Endereco_DevePermitirDefinirPropriedades()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Email = "test@test.com",
                Senha = "senha123",
                CPF = "123.456.789-00"
            };

            var endereco = new Endereco
            {
                Logradouro = "Av. Paulista",
                Numero = "1000",
                Complemento = "Sala 101",
                Bairro = "Bela Vista",
                Cidade = "São Paulo",
                Estado = "SP",
                CEP = "01310-100"
            };

            // Act
            usuario.Endereco = endereco;

            // Assert
            Assert.NotNull(usuario.Endereco);
            Assert.Equal("Av. Paulista", usuario.Endereco.Logradouro);
            Assert.Equal("1000", usuario.Endereco.Numero);
            Assert.Equal("Sala 101", usuario.Endereco.Complemento);
            Assert.Equal("Bela Vista", usuario.Endereco.Bairro);
            Assert.Equal("São Paulo", usuario.Endereco.Cidade);
            Assert.Equal("SP", usuario.Endereco.Estado);
            Assert.Equal("01310-100", usuario.Endereco.CEP);
        }
    }
}
