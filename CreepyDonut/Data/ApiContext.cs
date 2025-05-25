using CreepyDonut.Models;
using Microsoft.EntityFrameworkCore;

namespace CreepyDonut.Data
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<ProductShop> ProductShops { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
             .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Users <-> Cart (1-to-1)    
            modelBuilder.Entity<Users>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId)
                .IsRequired(false);

            // Cart <-> CartItem <-> Product (Many-to-Many via CartItem)    
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.CartId, ci.ProductId });

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            // Product <-> Category (Many-to-One)    
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product <-> Review (One-to-Many)    
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product <-> Shop (Many-to-Many via ProductShop)    
            modelBuilder.Entity<ProductShop>()
                .HasKey(ps => new { ps.ProductId, ps.ShopId });

            modelBuilder.Entity<ProductShop>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.ProductShops)
                .HasForeignKey(ps => ps.ProductId)
                 .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProductShop>()
                .HasOne(ps => ps.Shop)
                .WithMany(s => s.ProductShops)
                .HasForeignKey(ps => ps.ShopId)
                 .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
               .HasOne(r => r.User)
               .WithMany(u => u.Reviews)
               .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
               .HasOne(o => o.User)
               .WithMany(u => u.Orders)
               .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Cart)
                .WithOne(c => c.Order)
                .HasForeignKey<Order>(o => o.CartId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
