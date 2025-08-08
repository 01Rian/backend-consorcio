using Microsoft.EntityFrameworkCore;
using rian_p01_back.src.Models.Entities;

namespace rian_p01_back.src.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Administradora> Administradoras { get; set; }
        public DbSet<Consorcio> Consorcios { get; set; }
        public DbSet<Cotas> Cotas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().OwnsOne(u => u.Endereco);
            modelBuilder.Entity<Administradora>().OwnsOne(a => a.Endereco);
        }
    }
}