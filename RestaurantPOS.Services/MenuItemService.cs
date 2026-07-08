using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Services;

public class MenuItemService : IMenuItemService
{
    private readonly IMenuItemRepository _menuItemRepository;

    public MenuItemService()
    {
        _menuItemRepository = new MenuItemRepository();
    }

    public List<MenuItem> GetMenuItems() => _menuItemRepository.GetMenuItems();

    public MenuItem? GetMenuItemById(int menuItemId) => _menuItemRepository.GetMenuItemById(menuItemId);

    public bool SaveMenuItem(MenuItem menuItem) => _menuItemRepository.SaveMenuItem(menuItem);

    public bool UpdateMenuItem(MenuItem menuItem) => _menuItemRepository.UpdateMenuItem(menuItem);

    public bool DeleteMenuItem(int menuItemId) => _menuItemRepository.DeleteMenuItem(menuItemId);
}
