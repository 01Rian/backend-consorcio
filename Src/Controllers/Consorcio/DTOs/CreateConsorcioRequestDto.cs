using System.ComponentModel.DataAnnotations;
using rian_p01_back.src.Models.Entities;

public class CreateConsorcioRequestDto
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

    [Required(ErrorMessage = "Quantidade de cotas é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade de cotas deve ser maior que zero")]
    public int QuantidadeCotas { get; set; }

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

    public Consorcio ToEntity()
    {
        return new Consorcio
        {
            Nome = this.Nome,
            Codigo = this.Codigo,
            ValorBem = this.ValorBem,
            QuantidadeCotas = this.QuantidadeCotas,
            PrazoMeses = this.PrazoMeses,
            TaxaAdministracao = this.TaxaAdministracao,
            FundoReserva = this.FundoReserva,
            DataInicio = this.DataInicio ?? throw new ArgumentException("Data de início é obrigatória", nameof(DataInicio)),
            Ativo = this.Ativo ?? true
        };
    }
}