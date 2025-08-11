using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.src.Data.Repositories.Interfaces
{
    public interface ICotasRepository : IGenericRepository<Cotas>
    {
        Task<IEnumerable<Cotas>> GetByConsorcioAsync(int consorcioId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Cotas>> GetByUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default);
        Task<Cotas?> GetDetalhadaAsync(int id, CancellationToken cancellationToken = default);
    }
}
