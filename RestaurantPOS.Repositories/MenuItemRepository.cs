using RestaurantPOS.BusinessObjects;
using RestaurantPOS.DataAccessObjects;

namespace RestaurantPOS.Repositories;

public class MenuItemRepository : IMenuItemRepository
{
    public List<MenuItem> GetMenuItems() => MenuItemDAO.GetMenuItems();

    public MenuItem? GetMenuItemById(int menuItemId) => MenuItemDAO.GetMenuItemById(menuItemId);

    public bool SaveMenuItem(MenuItem menuItem) => MenuItemDAO.SaveMenuItem(menuItem);

    public bool UpdateMenuItem(MenuItem menuItem) => MenuItemDAO.UpdateMenuItem(menuItem);

    public bool DeleteMenuItem(int menuItemId) => MenuItemDAO.DeleteMenuItem(menuItemId);
}
