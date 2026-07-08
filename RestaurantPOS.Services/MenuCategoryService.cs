using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Services;

public class MenuCategoryService : IMenuCategoryService
{
    private readonly IMenuCategoryRepository _categoryRepository;

    public MenuCategoryService()
    {
        _categoryRepository = new MenuCategoryRepository();
    }

    public List<MenuCategory> GetCategories() => _categoryRepository.GetCategories();

    public bool SaveCategory(MenuCategory category) => _categoryRepository.SaveCategory(category);

    public bool DeleteCategory(int categoryId) => _categoryRepository.DeleteCategory(categoryId);
}
