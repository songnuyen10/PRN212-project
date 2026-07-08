using Microsoft.EntityFrameworkCore;
using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.DataAccessObjects;

// Initial data per Report 1 §6.3 ("Initialize SQL Server database with seed data:
// menu items, user roles"). The admin password hash/salt below is a fixed,
// pre-computed PBKDF2 pair (see docs/adr/0002) for "Admin@123" — computed once so
// migrations stay stable instead of regenerating a random salt on every model build.
public static class SeedData
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(new User
        {
            UserId = 1,
            Username = "admin",
            FullName = "Administrator",
            Role = UserRole.Admin,
            PasswordSalt = Convert.FromHexString("4bb4178a8c536507ab136708a9b56a27"),
            PasswordHash = Convert.FromHexString("89180b75ba85a62cb4f1469578e24dd9048e7b9504ad15180ddb656f3f46a230")
        });

        modelBuilder.Entity<RestaurantTable>().HasData(
            new RestaurantTable { TableId = 1, TableName = "T1", Capacity = 2, Status = TableStatus.Free },
            new RestaurantTable { TableId = 2, TableName = "T2", Capacity = 2, Status = TableStatus.Free },
            new RestaurantTable { TableId = 3, TableName = "T3", Capacity = 4, Status = TableStatus.Free },
            new RestaurantTable { TableId = 4, TableName = "T4", Capacity = 4, Status = TableStatus.Free },
            new RestaurantTable { TableId = 5, TableName = "T5", Capacity = 6, Status = TableStatus.Free },
            new RestaurantTable { TableId = 6, TableName = "T6", Capacity = 6, Status = TableStatus.Free });

        modelBuilder.Entity<MenuCategory>().HasData(
            new MenuCategory { MenuCategoryId = 1, CategoryName = "Appetizers" },
            new MenuCategory { MenuCategoryId = 2, CategoryName = "Main Course" },
            new MenuCategory { MenuCategoryId = 3, CategoryName = "Beverages" });

        modelBuilder.Entity<MenuItem>().HasData(
            new MenuItem { MenuItemId = 1, ItemName = "Spring Rolls", Price = 45000, MenuCategoryId = 1 },
            new MenuItem { MenuItemId = 2, ItemName = "Chicken Rice", Price = 55000, MenuCategoryId = 2 },
            new MenuItem { MenuItemId = 3, ItemName = "Beef Noodle Soup", Price = 65000, MenuCategoryId = 2 },
            new MenuItem { MenuItemId = 4, ItemName = "Iced Tea", Price = 15000, MenuCategoryId = 3 });

        modelBuilder.Entity<Ingredient>().HasData(
            new Ingredient { IngredientId = 1, IngredientName = "Chicken", Unit = "kg", QuantityInStock = 20, LowStockThreshold = 3 },
            new Ingredient { IngredientId = 2, IngredientName = "Rice", Unit = "kg", QuantityInStock = 30, LowStockThreshold = 5 },
            new Ingredient { IngredientId = 3, IngredientName = "Beef", Unit = "kg", QuantityInStock = 15, LowStockThreshold = 3 },
            new Ingredient { IngredientId = 4, IngredientName = "Rice Noodle", Unit = "kg", QuantityInStock = 20, LowStockThreshold = 4 },
            new Ingredient { IngredientId = 5, IngredientName = "Tea Leaves", Unit = "kg", QuantityInStock = 5, LowStockThreshold = 1 });

        modelBuilder.Entity<MenuItemIngredient>().HasData(
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 1, QuantityRequired = 0.3m },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 2, QuantityRequired = 0.2m },
            new MenuItemIngredient { MenuItemId = 3, IngredientId = 3, QuantityRequired = 0.25m },
            new MenuItemIngredient { MenuItemId = 3, IngredientId = 4, QuantityRequired = 0.2m },
            new MenuItemIngredient { MenuItemId = 4, IngredientId = 5, QuantityRequired = 0.02m });
    }
}
