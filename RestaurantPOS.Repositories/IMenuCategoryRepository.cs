using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Repositories;

public interface IMenuCategoryRepository
{
    List<MenuCategory> GetCategories();
    bool SaveCategory(MenuCategory category);
    bool DeleteCategory(int categoryId);
}
