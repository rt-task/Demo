using IdentityDemo.Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Dal
{
    public class AppDbContext : DbContext
    {
        public DbSet<CompanyEntity> Companies { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
