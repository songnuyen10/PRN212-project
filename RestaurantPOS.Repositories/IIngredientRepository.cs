using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Repositories;

public interface IIngredientRepository
{
    List<Ingredient> GetIngredients();
    bool SaveIngredient(Ingredient ingredient);
    bool UpdateIngredient(Ingredient ingredient);
    bool DeleteIngredient(int ingredientId);
}
