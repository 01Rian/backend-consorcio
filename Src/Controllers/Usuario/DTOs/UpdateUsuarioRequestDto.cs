using System.ComponentModel.DataAnnotations;
using rian_p01_back.src.Models.Entities;

public class UpdateUsuarioRequestDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    public string Email { get; set; } = string.Empty;

    [StringLength(255, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 255 caracteres")]
    public string? Senha { get; set; } 

    [Required(ErrorMessage = "CPF é obrigatório")]
    [StringLength(14, ErrorMessage = "CPF deve ter no máximo 14 caracteres")]
    public string CPF { get; set; } = string.Empty;

    [StringLength(15, ErrorMessage = "Telefone deve ter no máximo 15 caracteres")]
    public string? Telefone { get; set; }

    [StringLength(15, ErrorMessage = "Celular deve ter no máximo 15 caracteres")]
    public string? Celular { get; set; }

    public DateTime? DataNascimento { get; set; }

    public Endereco? Endereco { get; set; }

    public bool? Ativo { get; set; }
    public Usuario ToEntity(Usuario usuarioExistente)
    {
        usuarioExistente.Nome = this.Nome;
        usuarioExistente.Email = this.Email;
        usuarioExistente.Senha = this.Senha ?? usuarioExistente.Senha;
        usuarioExistente.CPF = this.CPF;
        usuarioExistente.Telefone = this.Telefone;
        usuarioExistente.Celular = this.Celular;
        usuarioExistente.DataNascimento = this.DataNascimento;
        usuarioExistente.Endereco = this.Endereco;
        usuarioExistente.Ativo = this.Ativo ?? usuarioExistente.Ativo;
        return usuarioExistente;
    }
}