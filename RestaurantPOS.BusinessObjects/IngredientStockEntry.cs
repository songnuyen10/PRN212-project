namespace RestaurantPOS.BusinessObjects;

public class IngredientStockEntry
{
    public int IngredientStockEntryId { get; set; }
    public int IngredientId { get; set; }
    public virtual Ingredient Ingredient { get; set; } = null!;
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public decimal QuantityAdded { get; set; }
    public DateTime ReceivedAt { get; set; }
    public string? Note { get; set; }
}
