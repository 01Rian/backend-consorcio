using Microsoft.EntityFrameworkCore;
using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.src.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Consorcio> Consorcios { get; set; }
        public DbSet<Cotas> Cotas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().OwnsOne(u => u.Endereco);

            modelBuilder.Entity<Cotas>(b =>
            {
                b.Property(e => e.ValorParcela).HasPrecision(18, 2);
                b.Property(e => e.ValorPago).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Consorcio>(b =>
            {
                b.Property(e => e.ValorBem).HasPrecision(18, 2);
                b.Property(e => e.TaxaAdministracao).HasPrecision(18, 2);
                b.Property(e => e.FundoReserva).HasPrecision(18, 2);
            });
        }
    }
}