using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Services;

public class IngredientService : IIngredientService
{
    private readonly IIngredientRepository _ingredientRepository;

    public IngredientService()
    {
        _ingredientRepository = new IngredientRepository();
    }

    // Test seam only — production code always uses the parameterless constructor.
    public IngredientService(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public List<Ingredient> GetIngredients() => _ingredientRepository.GetIngredients();

    public List<Ingredient> GetLowStockIngredients() =>
        _ingredientRepository.GetIngredients().Where(i => i.IsLowStock).ToList();

    public bool SaveIngredient(Ingredient ingredient) => _ingredientRepository.SaveIngredient(ingredient);

    public bool UpdateIngredient(Ingredient ingredient) => _ingredientRepository.UpdateIngredient(ingredient);

    public bool DeleteIngredient(int ingredientId) => _ingredientRepository.DeleteIngredient(ingredientId);
}
