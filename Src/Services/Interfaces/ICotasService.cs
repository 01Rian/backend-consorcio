using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.src.Services.Interfaces
{
    public interface ICotasService : IGenericService<Cotas>
    {
        Task<IEnumerable<Cotas>> GetByConsorcioAsync(int consorcioId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Cotas>> GetByUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default);
        Task<Cotas?> GetDetailedAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Cotas>> GetContemplatedAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Cotas>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<bool> ContemplateAsync(int cotaId, CancellationToken cancellationToken = default);
        Task<bool> RegisterPaymentAsync(int cotaId, decimal valorPago, CancellationToken cancellationToken = default);
    }
}