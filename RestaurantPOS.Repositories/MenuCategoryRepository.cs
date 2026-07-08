using RestaurantPOS.BusinessObjects;
using RestaurantPOS.DataAccessObjects;

namespace RestaurantPOS.Repositories;

public class MenuCategoryRepository : IMenuCategoryRepository
{
    public List<MenuCategory> GetCategories() => MenuCategoryDAO.GetCategories();

    public bool SaveCategory(MenuCategory category) => MenuCategoryDAO.SaveCategory(category);

    public bool DeleteCategory(int categoryId) => MenuCategoryDAO.DeleteCategory(categoryId);
}
