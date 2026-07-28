using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Tests;

public class FakeMenuCategoryRepository : IMenuCategoryRepository
{
    private readonly List<MenuCategory> _categories = new();

    public void Seed(MenuCategory category) => _categories.Add(category);

    public List<MenuCategory> GetCategories() => _categories;

    public bool SaveCategory(MenuCategory category) => true;

    public bool DeleteCategory(int categoryId) => true;
}
