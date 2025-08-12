using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.src.Data.Repositories.Interfaces
{
    public interface IAdministradoraRepository : IGenericRepository<Administradora>
    {
        Task<Administradora?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
    }
}
