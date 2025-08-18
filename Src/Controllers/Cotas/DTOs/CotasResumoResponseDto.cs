using rian_p01_back.src.Models.Entities;

public class CotasResumoResponseDto
{
    public int Id { get; set; }
    public string NumeroCota { get; set; } = string.Empty;
    public decimal ValorParcela { get; set; }
    public decimal ValorPago { get; set; }
    public int ParcelasPagas { get; set; }
    public bool Contemplada { get; set; }
    public StatusCota Status { get; set; }
    public bool Ativo { get; set; }

    public static CotasResumoResponseDto FromEntity(Cotas cota)
    {
        return new CotasResumoResponseDto
        {
            Id = cota.Id,
            NumeroCota = cota.NumeroCota,
            ValorParcela = cota.ValorParcela,
            ValorPago = cota.ValorPago,
            ParcelasPagas = cota.ParcelasPagas,
            Contemplada = cota.Contemplada,
            Status = cota.Status,
            Ativo = cota.Ativo,
        };
    }

    public static IEnumerable<CotasResumoResponseDto> FromEntities(IEnumerable<Cotas> cotas)
    {
        return cotas.Select(FromEntity);
    }
}
