using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Repositories;

public interface IMenuItemRepository
{
    List<MenuItem> GetMenuItems();
    MenuItem? GetMenuItemById(int menuItemId);
    bool SaveMenuItem(MenuItem menuItem);
    bool UpdateMenuItem(MenuItem menuItem);
    bool DeleteMenuItem(int menuItemId);
}
