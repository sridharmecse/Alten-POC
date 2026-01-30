using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) {}
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(p => p.ProductId);
                e.Property(p => p.Name).IsRequired().HasMaxLength(200);
                e.Property(p => p.Price).HasColumnType("decimal(18,2)");
                e.Property(p => p.StockQty).HasDefaultValue(0);
            });

            // Seed data
            modelBuilder.Entity<Product>().HasData(
                new Product{ ProductId = 1, Name = "Laptop", Price = 89999.00m, StockQty = 10, ImageUrl = null },
                new Product{ ProductId = 2, Name = "Phone", Price = 49999.00m, StockQty = 20, ImageUrl = null },
                new Product{ ProductId = 3, Name = "Headphones", Price = 2999.00m, StockQty = 50, ImageUrl = null }
            );
        }
    }
}
