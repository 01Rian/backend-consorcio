using System.ComponentModel.DataAnnotations;
using rian_p01_back.src.Models.Entities;

public class UpdateConsorcioRequestDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Código é obrigatório")]
    [StringLength(50, ErrorMessage = "Código deve ter no máximo 50 caracteres")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Valor do bem é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor do bem deve ser maior que zero")]
    public decimal ValorBem { get; set; }

    [Required(ErrorMessage = "Prazo em meses é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "Prazo em meses deve ser maior que zero")]
    public int PrazoMeses { get; set; }

    [Required(ErrorMessage = "Taxa de administração é obrigatória")]
    [Range(0, double.MaxValue, ErrorMessage = "Taxa de administração não pode ser negativa")]
    public decimal TaxaAdministracao { get; set; }

    [Required(ErrorMessage = "Fundo de reserva é obrigatório")]
    [Range(0, double.MaxValue, ErrorMessage = "Fundo de reserva não pode ser negativo")]
    public decimal FundoReserva { get; set; }

    [Required(ErrorMessage = "Data de início é obrigatória")]
    public DateTime? DataInicio { get; set; }

    public bool? Ativo { get; set; }

    public Consorcio ToEntity(Consorcio consorcioExistente)
    {
        consorcioExistente.Nome = this.Nome;
        consorcioExistente.Codigo = this.Codigo;
        consorcioExistente.ValorBem = this.ValorBem;
        consorcioExistente.PrazoMeses = this.PrazoMeses;
        consorcioExistente.TaxaAdministracao = this.TaxaAdministracao;
        consorcioExistente.FundoReserva = this.FundoReserva;
        consorcioExistente.DataInicio = this.DataInicio ?? consorcioExistente.DataInicio;
        consorcioExistente.Ativo = this.Ativo ?? consorcioExistente.Ativo;
        return consorcioExistente;
    }
}
