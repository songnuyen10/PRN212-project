using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Services;

public interface IIngredientService
{
    List<Ingredient> GetIngredients();
    List<Ingredient> GetLowStockIngredients();
    bool SaveIngredient(Ingredient ingredient);
    bool UpdateIngredient(Ingredient ingredient);
    bool DeleteIngredient(int ingredientId);
    bool ReceiveStock(int ingredientId, decimal quantity, int userId, string? note);
    List<IngredientStockEntry> GetStockEntriesByIngredient(int ingredientId);
}
