using rian_p01_back.src.Models.Entities;

public class ConsorcioResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public decimal ValorBem { get; set; }
    public int QuantidadeCotas { get; set; }
    public int PrazoMeses { get; set; }
    public decimal TaxaAdministracao { get; set; }
    public decimal FundoReserva { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataTermino { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public IEnumerable<CotasResumoResponseDto>? Cotas { get; set; }

    public static ConsorcioResponseDto FromEntity(Consorcio consorcio)
    {
        return new ConsorcioResponseDto
        {
            Id = consorcio.Id,
            Nome = consorcio.Nome,
            Codigo = consorcio.Codigo,
            ValorBem = consorcio.ValorBem,
            QuantidadeCotas = consorcio.QuantidadeCotas,
            PrazoMeses = consorcio.PrazoMeses,
            TaxaAdministracao = consorcio.TaxaAdministracao,
            FundoReserva = consorcio.FundoReserva,
            DataInicio = consorcio.DataInicio,
            DataTermino = consorcio.DataTermino,
            Ativo = consorcio.Ativo,
            DataCadastro = consorcio.DataCadastro,
            DataAtualizacao = consorcio.DataAtualizacao,
            Cotas = consorcio.Cotas?.Select(CotasResumoResponseDto.FromEntity)
        };
    }

    public static IEnumerable<ConsorcioResponseDto> FromEntities(IEnumerable<Consorcio> consorcios)
    {
        return consorcios.Select(FromEntity);
    }
}
