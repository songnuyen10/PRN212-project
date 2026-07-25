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

    public bool SaveIngredient(Ingredient ingredient)
    {
        if (IsDuplicateName(ingredient.IngredientName, excludeId: null)) return false;
        return _ingredientRepository.SaveIngredient(ingredient);
    }

    public bool UpdateIngredient(Ingredient ingredient)
    {
        if (IsDuplicateName(ingredient.IngredientName, excludeId: ingredient.IngredientId)) return false;
        return _ingredientRepository.UpdateIngredient(ingredient);
    }

    public bool DeleteIngredient(int ingredientId) => _ingredientRepository.DeleteIngredient(ingredientId);

    public bool ReceiveStock(int ingredientId, decimal quantity, int userId, string? note)
    {
        if (quantity <= 0) return false;
        return _ingredientRepository.ReceiveStock(ingredientId, quantity, userId, note);
    }

    public List<IngredientStockEntry> GetStockEntriesByIngredient(int ingredientId) =>
        _ingredientRepository.GetStockEntriesByIngredient(ingredientId);

    private bool IsDuplicateName(string name, int? excludeId) =>
        _ingredientRepository.GetIngredients().Any(i =>
            i.IngredientId != excludeId && string.Equals(i.IngredientName, name, StringComparison.OrdinalIgnoreCase));
}
