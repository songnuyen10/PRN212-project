using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.DataAccessObjects;

public class MenuCategoryDAO
{
    public static List<MenuCategory> GetCategories()
    {
        using var context = new AppDbContext();
        return context.MenuCategories.ToList();
    }

    public static bool SaveCategory(MenuCategory category)
    {
        using var context = new AppDbContext();
        try
        {
            context.MenuCategories.Add(category);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"{nameof(MenuCategoryDAO)}.{nameof(SaveCategory)}", ex);
            return false;
        }
    }

    public static bool DeleteCategory(int categoryId)
    {
        using var context = new AppDbContext();
        try
        {
            var category = context.MenuCategories.FirstOrDefault(c => c.MenuCategoryId == categoryId);
            if (category == null) return false;
            context.MenuCategories.Remove(category);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"{nameof(MenuCategoryDAO)}.{nameof(DeleteCategory)}", ex);
            return false;
        }
    }
}
