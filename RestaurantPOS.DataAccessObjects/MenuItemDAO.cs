using Microsoft.EntityFrameworkCore;
using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.DataAccessObjects;

public class MenuItemDAO
{
    public static List<MenuItem> GetMenuItems()
    {
        using var context = new AppDbContext();
        return context.MenuItems
            .Include(m => m.MenuCategory)
            .Include(m => m.MenuItemIngredients)
            .ToList();
    }

    public static MenuItem? GetMenuItemById(int menuItemId)
    {
        using var context = new AppDbContext();
        return context.MenuItems
            .Include(m => m.MenuCategory)
            .Include(m => m.MenuItemIngredients)
            .ThenInclude(mi => mi.Ingredient)
            .FirstOrDefault(m => m.MenuItemId == menuItemId);
    }

    public static bool SaveMenuItem(MenuItem menuItem)
    {
        using var context = new AppDbContext();
        try
        {
            context.MenuItems.Add(menuItem);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            return false;
        }
    }

    public static bool UpdateMenuItem(MenuItem menuItem)
    {
        using var context = new AppDbContext();
        try
        {
            context.MenuItems.Update(menuItem);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            return false;
        }
    }

    public static bool DeleteMenuItem(int menuItemId)
    {
        using var context = new AppDbContext();
        try
        {
            var menuItem = context.MenuItems.FirstOrDefault(m => m.MenuItemId == menuItemId);
            if (menuItem == null) return false;
            context.MenuItems.Remove(menuItem);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            return false;
        }
    }
}
