using RestaurantPOS.BusinessObjects;
using RestaurantPOS.DataAccessObjects;

namespace RestaurantPOS.Repositories;

public class IngredientRepository : IIngredientRepository
{
    public List<Ingredient> GetIngredients() => IngredientDAO.GetIngredients();

    public bool SaveIngredient(Ingredient ingredient) => IngredientDAO.SaveIngredient(ingredient);

    public bool UpdateIngredient(Ingredient ingredient) => IngredientDAO.UpdateIngredient(ingredient);

    public bool DeleteIngredient(int ingredientId) => IngredientDAO.DeleteIngredient(ingredientId);
}
