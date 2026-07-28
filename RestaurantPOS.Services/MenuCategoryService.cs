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

    // Test seam only — production code always uses the parameterless constructor.
    public MenuCategoryService(IMenuCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public List<MenuCategory> GetCategories() => _categoryRepository.GetCategories();

    public bool SaveCategory(MenuCategory category)
    {
        if (IsDuplicateName(category.CategoryName, excludeId: null)) return false;
        return _categoryRepository.SaveCategory(category);
    }

    public bool DeleteCategory(int categoryId) => _categoryRepository.DeleteCategory(categoryId);

    private bool IsDuplicateName(string name, int? excludeId) =>
        _categoryRepository.GetCategories().Any(c =>
            c.MenuCategoryId != excludeId && string.Equals(c.CategoryName, name, StringComparison.OrdinalIgnoreCase));
}
