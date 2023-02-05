using Microsoft.EntityFrameworkCore;

namespace RESTful_API_Olymp.Domain
{
    public class TypeContext : DbContext
    {
        public DbSet<TypeContext> Types { get; set; }

        public TypeContext(DbContextOptions<TypeContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
