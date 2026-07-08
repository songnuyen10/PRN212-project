using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Tests;

public class FakeIngredientRepository : IIngredientRepository
{
    private readonly List<Ingredient> _ingredients = new();

    public void Seed(Ingredient ingredient) => _ingredients.Add(ingredient);

    public List<Ingredient> GetIngredients() => _ingredients;

    public bool SaveIngredient(Ingredient ingredient) => true;

    public bool UpdateIngredient(Ingredient ingredient) => true;

    public bool DeleteIngredient(int ingredientId) => true;
}
