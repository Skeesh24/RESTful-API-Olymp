using Microsoft.EntityFrameworkCore;
using RESTful_API_Olymp.Domain.Entities;

namespace RESTful_API_Olymp.Domain
{
    public class DataContext : DbContext
    {
        public DbSet<AccountEntity> Accounts { get; set; }
        public DbSet<AnimalEntity> Animals { get; set; }
        public DbSet<LocationEntity> Locations { get; set; }
        public DbSet<PointEntity> Points { get; set; }
        public DbSet<TypeEntity> Types { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
