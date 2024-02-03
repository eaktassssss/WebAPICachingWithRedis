using Microsoft.EntityFrameworkCore;
using WebAPICachingWithRedis.Entities;

namespace WebAPICachingWithRedis.Persistance.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions):base(dbContextOptions)
        {
        }
        public DbSet<Product> Products { get; set; }
    }
}
