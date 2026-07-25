using Microsoft.EntityFrameworkCore;
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
            AppLogger.LogError($"{nameof(IngredientDAO)}.{nameof(SaveIngredient)}", ex);
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
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"{nameof(IngredientDAO)}.{nameof(UpdateIngredient)}", ex);
            return false;
        }
    }

    public static bool ReceiveStock(int ingredientId, decimal quantity, int userId, string? note)
    {
        using var context = new AppDbContext();
        try
        {
            var ingredient = context.Ingredients.FirstOrDefault(i => i.IngredientId == ingredientId);
            if (ingredient == null) return false;

            ingredient.QuantityInStock += quantity;
            context.IngredientStockEntries.Add(new IngredientStockEntry
            {
                IngredientId = ingredientId,
                UserId = userId,
                QuantityAdded = quantity,
                ReceivedAt = DateTime.Now,
                Note = note
            });
            context.SaveChanges();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"{nameof(IngredientDAO)}.{nameof(ReceiveStock)}", ex);
            return false;
        }
    }

    public static List<IngredientStockEntry> GetStockEntriesByIngredient(int ingredientId)
    {
        using var context = new AppDbContext();
        return context.IngredientStockEntries
            .Include(e => e.User)
            .Where(e => e.IngredientId == ingredientId)
            .OrderByDescending(e => e.ReceivedAt)
            .ToList();
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
            AppLogger.LogError($"{nameof(IngredientDAO)}.{nameof(DeleteIngredient)}", ex);
            return false;
        }
    }
}
