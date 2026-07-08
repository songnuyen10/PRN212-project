using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.DataAccessObjects;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<RestaurantTable> RestaurantTables { get; set; } = null!;
    public virtual DbSet<MenuCategory> MenuCategories { get; set; } = null!;
    public virtual DbSet<MenuItem> MenuItems { get; set; } = null!;
    public virtual DbSet<Ingredient> Ingredients { get; set; } = null!;
    public virtual DbSet<MenuItemIngredient> MenuItemIngredients { get; set; } = null!;
    public virtual DbSet<Order> Orders { get; set; } = null!;
    public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
    public virtual DbSet<Payment> Payments { get; set; } = null!;
    public virtual DbSet<Shift> Shifts { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }
    }

    private static string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        return configuration["ConnectionStrings:DefaultConnectionString"]
            ?? "Server=(localdb)\\mssqllocaldb;Database=RestaurantPOSDb;Trusted_Connection=True;TrustServerCertificate=True;";
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).HasMaxLength(50);
            entity.Property(u => u.FullName).HasMaxLength(100);
        });

        modelBuilder.Entity<RestaurantTable>(entity =>
        {
            // TableId doesn't match the RestaurantTable{Id} convention EF Core looks
            // for, so the primary key needs to be declared explicitly.
            entity.HasKey(t => t.TableId);
            entity.Property(t => t.TableName).HasMaxLength(20);
        });

        modelBuilder.Entity<MenuCategory>(entity =>
        {
            entity.Property(c => c.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.Property(i => i.ItemName).HasMaxLength(150);
            entity.Property(i => i.Price).HasColumnType("money");

            entity.HasOne(i => i.MenuCategory)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(i => i.MenuCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.Property(i => i.IngredientName).HasMaxLength(100);
            entity.Property(i => i.Unit).HasMaxLength(20);
            entity.Property(i => i.QuantityInStock).HasPrecision(10, 3);
            entity.Property(i => i.LowStockThreshold).HasPrecision(10, 3);
        });

        // MenuItemIngredient: explicit join entity (not an implicit many-to-many) so
        // recipe quantity can live on the relationship itself — see Report 2 §2.2.
        modelBuilder.Entity<MenuItemIngredient>(entity =>
        {
            entity.HasKey(mi => new { mi.MenuItemId, mi.IngredientId });
            entity.Property(mi => mi.QuantityRequired).HasPrecision(10, 3);

            entity.HasOne(mi => mi.MenuItem)
                .WithMany(m => m.MenuItemIngredients)
                .HasForeignKey(mi => mi.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(mi => mi.Ingredient)
                .WithMany(i => i.MenuItemIngredients)
                .HasForeignKey(mi => mi.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(o => o.RowVersion).IsRowVersion();

            entity.HasOne(o => o.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(o => o.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.OpenedByUser)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.OpenedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Backing field for the encapsulated OrderItems collection (composition:
            // an OrderItem cannot exist without its parent Order).
            entity.Metadata.FindNavigation(nameof(Order.OrderItems))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(oi => oi.UnitPrice).HasColumnType("money");

            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(oi => oi.MenuItem)
                .WithMany(m => m.OrderItems)
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(p => p.AmountPaid).HasColumnType("money");

            // Order–Payment: required 1-to-0..1 via a unique FK on Payments.OrderId.
            entity.HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.CashierUser)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.CashierUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Which shift a payment counts toward for cash reconciliation — optional,
            // a payment can be taken with no shift open.
            entity.HasOne(p => p.Shift)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.ShiftId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.Property(s => s.OpeningCash).HasColumnType("money");
            entity.Property(s => s.ClosingCash).HasColumnType("money");

            entity.HasOne(s => s.User)
                .WithMany(u => u.Shifts)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData.Apply(modelBuilder);
    }
}
