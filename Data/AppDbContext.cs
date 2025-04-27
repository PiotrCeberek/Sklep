using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Projekt.Models;
using Projekt.Models.ViewModels;

namespace Projekt.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ArticleComment> ArticleComments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<ItemOrder> ItemOrders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<PasswordResetCodeModel> PasswordResetCodes { get; set; }
        public DbSet<WalutyAPI> Waluty { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<ItemOrder>()
                .Property(io => io.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Promotion>()
                .Property(p => p.Discount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.ItemOrders)
                .WithOne(io => io.Order)
                .HasForeignKey(io => io.OrderId);

            modelBuilder.Entity<ItemOrder>()
                .HasOne(io => io.Product)
                .WithMany(p => p.ItemOrders)
                .HasForeignKey(io => io.ProductId);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Product)
                .WithMany(p => p.Favorites)
                .HasForeignKey(f => f.ProductId);

            modelBuilder.Entity<ArticleComment>()
                .HasOne(ac => ac.User)
                .WithMany(u => u.ArticleComments)
                .HasForeignKey(ac => ac.UserId);

            modelBuilder.Entity<ArticleComment>()
                .HasOne(ac => ac.Product)
                .WithMany(p => p.ArticleComments)
                .HasForeignKey(ac => ac.ProductId);

            modelBuilder.Entity<History>()
                .HasOne(h => h.User)
                .WithMany(u => u.Histories)
                .HasForeignKey(h => h.UserId);

            modelBuilder.Entity<History>()
                .HasOne(h => h.Order)
                .WithMany(o => o.Histories)
                .HasForeignKey(h => h.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Promotion>()
                .HasOne(p => p.Product)
                .WithMany(p => p.Promotions)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany()
                .HasForeignKey(ci => ci.UserId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);

            modelBuilder.Entity<PasswordResetCodeModel>()
                .HasOne(prc => prc.User)
                .WithMany()
                .HasForeignKey(prc => prc.UserId)
                .OnDelete(DeleteBehavior.Cascade);       

            

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Order)
                .WithMany()
                .HasForeignKey(n => n.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}