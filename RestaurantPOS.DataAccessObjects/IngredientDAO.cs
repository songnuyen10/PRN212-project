using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.DataAccessObjects;

public class IngredientDAO
{
    public static List<Ingredient> GetIngredients()
    {
        using var context = new AppDbContext();
        return context.Ingredients.ToList();
    }

    public static bool SaveIngredient(Ingredient ingredient)
    {
        using var context = new AppDbContext();
        try
        {
            context.Ingredients.Add(ingredient);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            return false;
        }
    }

    public static bool UpdateIngredient(Ingredient ingredient)
    {
        using var context = new AppDbContext();
        try
        {
            context.Ingredients.Update(ingredient);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            return false;
        }
    }

    public static bool DeleteIngredient(int ingredientId)
    {
        using var context = new AppDbContext();
        try
        {
            var ingredient = context.Ingredients.FirstOrDefault(i => i.IngredientId == ingredientId);
            if (ingredient == null) return false;
            context.Ingredients.Remove(ingredient);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            return false;
        }
    }
}
