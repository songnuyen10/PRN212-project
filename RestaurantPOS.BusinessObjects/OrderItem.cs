namespace RestaurantPOS.BusinessObjects;

public class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    public int MenuItemId { get; set; }
    public virtual MenuItem MenuItem { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public OrderItemStatus Status { get; set; }
}
