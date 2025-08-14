using rian_p01_back.src.Models.Entities;

public class UsuarioResumoResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Celular { get; set; }
    public DateTime? DataNascimento { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataAtualizacao { get; set; }

    public static UsuarioResumoResponseDto FromEntity(Usuario usuario)
    {
        return new UsuarioResumoResponseDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            CPF = usuario.CPF,
            Telefone = usuario.Telefone,
            Celular = usuario.Celular,
            DataNascimento = usuario.DataNascimento,
            Ativo = usuario.Ativo,
            DataCadastro = usuario.DataCadastro,
            DataAtualizacao = usuario.DataAtualizacao
        };
    }

    public static IEnumerable<UsuarioResumoResponseDto> FromEntities(IEnumerable<Usuario> usuarios)
    {
        return usuarios.Select(FromEntity);
    }
}
