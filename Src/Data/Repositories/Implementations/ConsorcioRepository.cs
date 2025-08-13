using Microsoft.EntityFrameworkCore;
using rian_p01_back.src.Data.Repositories.Interfaces;
using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.src.Data.Repositories.Implementations
{
    public class ConsorcioRepository : GenericRepository<Consorcio>, IConsorcioRepository
    {
        public ConsorcioRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Consorcio?> GetWithCotasAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Include(c => c.Cotas)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }
    }
}
