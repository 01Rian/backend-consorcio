using System.ComponentModel.DataAnnotations;
using rian_p01_back.src.Models.Entities;

public class CreateUsuarioRequestDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 255 caracteres")]
    public string Senha { get; set; } = string.Empty;

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
    public Usuario ToEntity()
    {
        return new Usuario
        {
            Nome = this.Nome,
            Email = this.Email,
            Senha = this.Senha,
            CPF = this.CPF,
            Telefone = this.Telefone,
            Celular = this.Celular,
            DataNascimento = this.DataNascimento,
            Endereco = this.Endereco,
            Ativo = this.Ativo ?? true
        };
    }
}