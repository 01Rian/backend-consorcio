using Microsoft.EntityFrameworkCore;
using rian_p01_back.src.Data.Repositories.Implementations;
using rian_p01_back.src.Data;

namespace rian_p01_back.Tests.Repositories
{

    public class TestDbContext : ApplicationDbContext
    {
        public TestDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<TestEntity> TestEntities { get; set; }
    }

    public abstract class RepositoryTestBase : IDisposable
    {
        protected TestDbContext Context { get; private set; }
        protected GenericRepository<TestEntity> Repository { get; private set; }

        protected RepositoryTestBase()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new TestDbContext(options);
            Repository = new GenericRepository<TestEntity>(Context);
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}