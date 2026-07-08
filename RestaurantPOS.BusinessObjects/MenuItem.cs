namespace RestaurantPOS.BusinessObjects;

public class MenuItem
{
    public int MenuItemId { get; set; }
    public string ItemName { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;

    public int MenuCategoryId { get; set; }
    public virtual MenuCategory MenuCategory { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<MenuItemIngredient> MenuItemIngredients { get; set; } = new List<MenuItemIngredient>();

    public override string ToString() => ItemName;
}
