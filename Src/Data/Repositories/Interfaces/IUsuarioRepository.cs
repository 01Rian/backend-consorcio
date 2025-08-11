using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.src.Data.Repositories.Interfaces
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Usuario?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);
    }
}
