using rian_p01_back.src.Models.Entities;

public class ConsorcioResumoResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public decimal ValorBem { get; set; }
    public int QuantidadeCotas { get; set; }
    public int PrazoMeses { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataTermino { get; set; }
    public bool Ativo { get; set; }

    public static ConsorcioResumoResponseDto FromEntity(Consorcio consorcio)
    {
        return new ConsorcioResumoResponseDto
        {
            Id = consorcio.Id,
            Nome = consorcio.Nome,
            Codigo = consorcio.Codigo,
            ValorBem = consorcio.ValorBem,
            QuantidadeCotas = consorcio.QuantidadeCotas,
            PrazoMeses = consorcio.PrazoMeses,
            DataInicio = consorcio.DataInicio,
            DataTermino = consorcio.DataTermino,
            Ativo = consorcio.Ativo
        };
    }

    public static IEnumerable<ConsorcioResumoResponseDto> FromEntities(IEnumerable<Consorcio> consorcios)
    {
        return consorcios.Select(FromEntity);
    }
}
