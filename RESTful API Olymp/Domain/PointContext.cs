using Microsoft.EntityFrameworkCore;

namespace RESTful_API_Olymp.Domain
{
    public class PointContext : DbContext
    {
        public DbSet<PointContext> Points { get; set; }

        public PointContext(DbContextOptions<PointContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
