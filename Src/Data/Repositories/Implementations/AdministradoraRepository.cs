using Microsoft.EntityFrameworkCore;
using rian_p01_back.src.Data.Repositories.Interfaces;
using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.src.Data.Repositories.Implementations
{
    public class AdministradoraRepository : GenericRepository<Administradora>, IAdministradoraRepository
    {
        public AdministradoraRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Administradora?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(a => a.CNPJ == cnpj, cancellationToken);
        }
    }
}
