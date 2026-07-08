namespace RestaurantPOS.BusinessObjects;

// Recipe junction: how much of each Ingredient a MenuItem consumes per unit sold.
public class MenuItemIngredient
{
    public int MenuItemId { get; set; }
    public virtual MenuItem MenuItem { get; set; } = null!;

    public int IngredientId { get; set; }
    public virtual Ingredient Ingredient { get; set; } = null!;

    public decimal QuantityRequired { get; set; }
}
