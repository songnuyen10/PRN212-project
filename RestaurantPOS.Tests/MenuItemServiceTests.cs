using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using Xunit;

namespace RestaurantPOS.Tests;

public class MenuItemServiceTests
{
    [Fact]
    public void SaveMenuItem_ReturnsFalse_WhenNameAlreadyExists()
    {
        var repository = new FakeMenuItemRepository();
        repository.Seed(new MenuItem { MenuItemId = 1, ItemName = "Pho Bo" });
        var service = new MenuItemService(repository);

        var result = service.SaveMenuItem(new MenuItem { ItemName = "pho bo" });

        Assert.False(result);
    }

    [Fact]
    public void UpdateMenuItem_ReturnsFalse_WhenNameMatchesAnotherItem()
    {
        var repository = new FakeMenuItemRepository();
        repository.Seed(new MenuItem { MenuItemId = 1, ItemName = "Pho Bo" });
        repository.Seed(new MenuItem { MenuItemId = 2, ItemName = "Bun Cha" });
        var service = new MenuItemService(repository);

        var result = service.UpdateMenuItem(new MenuItem { MenuItemId = 2, ItemName = "Pho Bo" });

        Assert.False(result);
    }

    [Fact]
    public void UpdateMenuItem_Succeeds_WhenNameUnchangedOnSelf()
    {
        var repository = new FakeMenuItemRepository();
        repository.Seed(new MenuItem { MenuItemId = 1, ItemName = "Pho Bo" });
        var service = new MenuItemService(repository);

        var result = service.UpdateMenuItem(new MenuItem { MenuItemId = 1, ItemName = "Pho Bo" });

        Assert.True(result);
    }
}
