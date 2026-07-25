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

    [Fact]
    public void SaveIngredient_ReturnsFalse_WhenNameAlreadyExists()
    {
        var repository = new FakeIngredientRepository();
        repository.Seed(new Ingredient { IngredientId = 1, IngredientName = "Chicken" });
        var service = new IngredientService(repository);

        var result = service.SaveIngredient(new Ingredient { IngredientName = "chicken" });

        Assert.False(result);
    }

    [Fact]
    public void UpdateIngredient_ReturnsFalse_WhenNameMatchesAnotherIngredient()
    {
        var repository = new FakeIngredientRepository();
        repository.Seed(new Ingredient { IngredientId = 1, IngredientName = "Chicken" });
        repository.Seed(new Ingredient { IngredientId = 2, IngredientName = "Rice" });
        var service = new IngredientService(repository);

        var result = service.UpdateIngredient(new Ingredient { IngredientId = 2, IngredientName = "Chicken" });

        Assert.False(result);
    }

    [Fact]
    public void UpdateIngredient_Succeeds_WhenNameUnchangedOnSelf()
    {
        var repository = new FakeIngredientRepository();
        repository.Seed(new Ingredient { IngredientId = 1, IngredientName = "Chicken" });
        var service = new IngredientService(repository);

        var result = service.UpdateIngredient(new Ingredient { IngredientId = 1, IngredientName = "Chicken" });

        Assert.True(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ReceiveStock_ReturnsFalse_WhenQuantityIsNotPositive(decimal quantity)
    {
        var service = new IngredientService(new FakeIngredientRepository());

        var result = service.ReceiveStock(ingredientId: 1, quantity: quantity, userId: 1, note: null);

        Assert.False(result);
    }
}
