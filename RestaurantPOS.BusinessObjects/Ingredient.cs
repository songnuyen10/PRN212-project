namespace RestaurantPOS.BusinessObjects;

public class Ingredient
{
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = null!;
    public decimal QuantityInStock { get; set; }
    public string Unit { get; set; } = null!;
    public decimal LowStockThreshold { get; set; }

    public virtual ICollection<MenuItemIngredient> MenuItemIngredients { get; set; } = new List<MenuItemIngredient>();

    public bool IsLowStock => QuantityInStock <= LowStockThreshold;

    public override string ToString() => IngredientName;
}
