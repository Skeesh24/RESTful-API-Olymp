using Microsoft.EntityFrameworkCore;
using RESTful_API_Olymp.Domain.Entities;

namespace RESTful_API_Olymp
{
    public class DataContext : DbContext
    {
        public DbSet<AnimalEntity>? Animals { get; set; }
        public DbSet<LocationEntity>? Locations { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public DbSet<T> DbSet<T>() where T : class
        {
            return Set<T>();
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return Set<T>();
        }
    }
}
