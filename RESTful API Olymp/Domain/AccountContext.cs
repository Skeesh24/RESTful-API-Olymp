using Microsoft.EntityFrameworkCore;
using RESTful_API_Olymp.Domain.Entities;

namespace RESTful_API_Olymp.Domain
{
    public class AccountContext : DbContext
    {
        public DbSet<AccountEntity> Accounts { get; set; }

        public AccountContext(DbContextOptions<AccountContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
