using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using Xunit;

namespace RestaurantPOS.Tests;

public class MenuCategoryServiceTests
{
    [Fact]
    public void SaveCategory_ReturnsFalse_WhenNameAlreadyExists()
    {
        var repository = new FakeMenuCategoryRepository();
        repository.Seed(new MenuCategory { MenuCategoryId = 1, CategoryName = "Mon chinh" });
        var service = new MenuCategoryService(repository);

        var result = service.SaveCategory(new MenuCategory { CategoryName = "mon chinh" });

        Assert.False(result);
    }

    [Fact]
    public void SaveCategory_Succeeds_WhenNameIsNew()
    {
        var repository = new FakeMenuCategoryRepository();
        repository.Seed(new MenuCategory { MenuCategoryId = 1, CategoryName = "Mon chinh" });
        var service = new MenuCategoryService(repository);

        var result = service.SaveCategory(new MenuCategory { CategoryName = "Trang mieng" });

        Assert.True(result);
    }
}
