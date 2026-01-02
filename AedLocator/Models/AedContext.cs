using Microsoft.EntityFrameworkCore;

namespace AedLocator.Models
{
    public class AedContext : DbContext
    {
        public AedContext(DbContextOptions<AedContext> options) : base(options)
        {
        }

        public DbSet<Aed> Aeds { get; set; } = null!;
    }
}