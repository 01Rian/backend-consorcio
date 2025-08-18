using rian_p01_back.src.Models.Entities;

public class CotasResponseDto
{
    public int Id { get; set; }
    public string NumeroCota { get; set; } = string.Empty;
    public decimal ValorParcela { get; set; }
    public decimal ValorPago { get; set; }
    public int ParcelasPagas { get; set; }
    public bool Contemplada { get; set; }
    public DateTime? DataContemplacao { get; set; }
    public StatusCota Status { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public int ConsorcioId { get; set; }
    public int? UsuarioId { get; set; }

    public static CotasResponseDto FromEntity(Cotas cota)
    {
        return new CotasResponseDto
        {
            Id = cota.Id,
            NumeroCota = cota.NumeroCota,
            ValorParcela = cota.ValorParcela,
            ValorPago = cota.ValorPago,
            ParcelasPagas = cota.ParcelasPagas,
            Contemplada = cota.Contemplada,
            DataContemplacao = cota.DataContemplacao,
            Status = cota.Status,
            Ativo = cota.Ativo,
            DataCadastro = cota.DataCadastro,
            DataAtualizacao = cota.DataAtualizacao,
            ConsorcioId = cota.ConsorcioId,
            UsuarioId = cota.UsuarioId
        };
    }

    public static IEnumerable<CotasResponseDto> FromEntities(IEnumerable<Cotas> cotas)
    {
        return cotas.Select(FromEntity);
    }
}
