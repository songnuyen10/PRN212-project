using RestaurantPOS.BusinessObjects;
using RestaurantPOS.DataAccessObjects;

namespace RestaurantPOS.Repositories;

public class IngredientRepository : IIngredientRepository
{
    public List<Ingredient> GetIngredients() => IngredientDAO.GetIngredients();

    public bool SaveIngredient(Ingredient ingredient) => IngredientDAO.SaveIngredient(ingredient);

    public bool UpdateIngredient(Ingredient ingredient) => IngredientDAO.UpdateIngredient(ingredient);

    public bool DeleteIngredient(int ingredientId) => IngredientDAO.DeleteIngredient(ingredientId);

    public bool ReceiveStock(int ingredientId, decimal quantity, int userId, string? note) =>
        IngredientDAO.ReceiveStock(ingredientId, quantity, userId, note);

    public List<IngredientStockEntry> GetStockEntriesByIngredient(int ingredientId) =>
        IngredientDAO.GetStockEntriesByIngredient(ingredientId);
}
