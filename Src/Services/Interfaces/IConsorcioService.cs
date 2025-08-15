using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.src.Services.Interfaces
{
    public interface IConsorcioService : IGenericService<Consorcio>
    {
        Task<Consorcio?> GetWithCotasAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Consorcio>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<bool> AssignCotaToUsuarioAsync(int cotaId, int usuarioId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Cotas>> GetAvailableCotasAsync(int consorcioId, CancellationToken cancellationToken = default);
    }
}