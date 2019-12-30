using Microsoft.EntityFrameworkCore;
using Shop.Data.Entities;

namespace Shop.Data
{
    public class ShopDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public ShopDbContext(DbContextOptions<ShopDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Applies all the configurations for entities.
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
