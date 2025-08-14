using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.src.Services.Interfaces
{
    public interface IUsuarioService : IGenericService<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Usuario?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);
        bool ValidatePassword(Usuario usuario, string password);
    }
}
