using Microsoft.EntityFrameworkCore;
using RESTful_API_Olymp.Domain.Entities;

namespace RESTful_API_Olymp.Domain
{
    public class LocationContext : DbContext
    {
        public DbSet<LocationContext> Locations { get; set; }

        public LocationContext(DbContextOptions<LocationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
