using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Services;

public interface IMenuCategoryService
{
    List<MenuCategory> GetCategories();
    bool SaveCategory(MenuCategory category);
    bool DeleteCategory(int categoryId);
}
