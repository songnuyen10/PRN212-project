using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Services;

public interface IMenuItemService
{
    List<MenuItem> GetMenuItems();
    MenuItem? GetMenuItemById(int menuItemId);
    bool SaveMenuItem(MenuItem menuItem);
    bool UpdateMenuItem(MenuItem menuItem);
    bool DeleteMenuItem(int menuItemId);
}
