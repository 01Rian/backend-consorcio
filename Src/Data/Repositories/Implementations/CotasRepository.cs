using Microsoft.EntityFrameworkCore;
using rian_p01_back.src.Data.Repositories.Interfaces;
using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.src.Data.Repositories.Implementations
{
    public class CotasRepository : GenericRepository<Cotas>, ICotasRepository
    {
        public CotasRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Cotas>> GetByConsorcioAsync(int consorcioId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(c => c.ConsorcioId == consorcioId)
                .Include(c => c.Consorcio)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Cotas>> GetByUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Cotas?> GetDetalhadaAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(c => c.Consorcio)
                .Include(c => c.Usuario!)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }
    }
}
