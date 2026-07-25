using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Repositories;

public interface IIngredientRepository
{
    List<Ingredient> GetIngredients();
    bool SaveIngredient(Ingredient ingredient);
    bool UpdateIngredient(Ingredient ingredient);
    bool DeleteIngredient(int ingredientId);
    bool ReceiveStock(int ingredientId, decimal quantity, int userId, string? note);
    List<IngredientStockEntry> GetStockEntriesByIngredient(int ingredientId);
}
