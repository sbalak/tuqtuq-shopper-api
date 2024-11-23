using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Shopper.Data
{
    public class ShopperContext : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public ShopperContext(DbContextOptions<ShopperContext> options) : base(options)
        {
        }
                public DbSet<Category> Categories { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().Property(m => m.FirstName).HasMaxLength(50);
            builder.Entity<User>().Property(m => m.LastName).HasMaxLength(50);
            builder.Entity<User>().Property(m => m.Latitude).HasMaxLength(200);
            builder.Entity<User>().Property(m => m.Longitude).HasMaxLength(200);

            builder.Entity<User>(b => { b.ToTable("Users"); });
            builder.Entity<UserClaim>(b => { b.ToTable("UserClaims"); });
            builder.Entity<UserLogin>(b => { b.ToTable("UserLogins"); });
            builder.Entity<UserToken>(b => { b.ToTable("UserTokens"); });
            builder.Entity<Role>(b => { b.ToTable("Roles"); });
            builder.Entity<RoleClaim>(b => { b.ToTable("RoleClaims"); });
            builder.Entity<UserRole>(b => { b.ToTable("UserRoles"); });

            builder.Entity<CartItem>()
            .HasOne(r => r.FoodItem)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<OrderItem>()
            .HasOne(r => r.FoodItem)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FoodItem>()
            .HasOne(r => r.Restaurant)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Order>()
            .Property(r => r.TaxableAmount)
            .HasPrecision(18, 2);

            builder.Entity<Order>()
            .Property(r => r.Amount)
            .HasPrecision(18, 2);

            builder.Entity<Order>()
            .Property(r => r.PrimaryTaxAmount)
            .HasPrecision(18, 2);

            builder.Entity<Order>()
            .Property(r => r.SecondaryTaxAmount)
            .HasPrecision(18, 2);

            builder.Entity<OrderItem>()
            .Property(r => r.TaxableAmount)
            .HasPrecision(18, 2);

            builder.Entity<OrderItem>()
            .Property(r => r.Amount)
            .HasPrecision(18, 2);

            builder.Entity<FoodItem>()
            .Property(r => r.Price)
            .HasPrecision(18, 2);

            builder.Entity<Restaurant>()
            .Property(r => r.PrimaryTaxRate)
            .HasPrecision(18, 2);

            builder.Entity<Restaurant>()
            .Property(r => r.SecondaryTaxRate)
            .HasPrecision(18, 2);

        }
    }
}