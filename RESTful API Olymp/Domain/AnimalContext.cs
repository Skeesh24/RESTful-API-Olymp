using Microsoft.EntityFrameworkCore;
using RESTful_API_Olymp.Domain.Entities;

namespace RESTful_API_Olymp.Domain
{
    public class AnimalContext : DbContext
    {
        public DbSet<AnimalEntity> Animals { get; set; }

        public AnimalContext(DbContextOptions<AnimalContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
