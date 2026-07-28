using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Tests;

public class FakeMenuItemRepository : IMenuItemRepository
{
    private readonly List<MenuItem> _menuItems = new();

    public void Seed(MenuItem menuItem) => _menuItems.Add(menuItem);

    public List<MenuItem> GetMenuItems() => _menuItems;

    public MenuItem? GetMenuItemById(int menuItemId) => _menuItems.FirstOrDefault(i => i.MenuItemId == menuItemId);

    public bool SaveMenuItem(MenuItem menuItem) => true;

    public bool UpdateMenuItem(MenuItem menuItem) => true;

    public bool DeleteMenuItem(int menuItemId) => true;
}
