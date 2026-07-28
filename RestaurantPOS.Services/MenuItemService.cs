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

    // Test seam only — production code always uses the parameterless constructor.
    public MenuItemService(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public List<MenuItem> GetMenuItems() => _menuItemRepository.GetMenuItems();

    public MenuItem? GetMenuItemById(int menuItemId) => _menuItemRepository.GetMenuItemById(menuItemId);

    public bool SaveMenuItem(MenuItem menuItem)
    {
        if (IsDuplicateName(menuItem.ItemName, excludeId: null)) return false;
        return _menuItemRepository.SaveMenuItem(menuItem);
    }

    public bool UpdateMenuItem(MenuItem menuItem)
    {
        if (IsDuplicateName(menuItem.ItemName, excludeId: menuItem.MenuItemId)) return false;
        return _menuItemRepository.UpdateMenuItem(menuItem);
    }

    public bool DeleteMenuItem(int menuItemId) => _menuItemRepository.DeleteMenuItem(menuItemId);

    private bool IsDuplicateName(string name, int? excludeId) =>
        _menuItemRepository.GetMenuItems().Any(i =>
            i.MenuItemId != excludeId && string.Equals(i.ItemName, name, StringComparison.OrdinalIgnoreCase));
}
