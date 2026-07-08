using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using Xunit;

namespace RestaurantPOS.Tests;

public class IngredientServiceTests
{
    [Fact]
    public void GetLowStockIngredients_ReturnsOnlyIngredientsAtOrBelowThreshold()
    {
        var repository = new FakeIngredientRepository();
        repository.Seed(new Ingredient { IngredientId = 1, IngredientName = "Chicken", QuantityInStock = 1, LowStockThreshold = 3 });
        repository.Seed(new Ingredient { IngredientId = 2, IngredientName = "Rice", QuantityInStock = 30, LowStockThreshold = 5 });
        var service = new IngredientService(repository);

        var result = service.GetLowStockIngredients();

        Assert.Single(result);
        Assert.Equal("Chicken", result[0].IngredientName);
    }
}
