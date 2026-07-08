using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.WpfApp.ViewModels;

// A menu item picked in the cart but not yet sent to the kitchen (not persisted).
public class CartLine
{
    public MenuItem MenuItem { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Subtotal => MenuItem.Price * Quantity;
}
