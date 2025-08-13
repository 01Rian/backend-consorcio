using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.src.Data.Repositories.Interfaces
{
    public interface IConsorcioRepository : IGenericRepository<Consorcio>
    {
        Task<Consorcio?> GetWithCotasAsync(int id, CancellationToken cancellationToken = default);
    }
}
