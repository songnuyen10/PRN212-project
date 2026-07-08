namespace RestaurantPOS.BusinessObjects;

public class Order
{
    public int OrderId { get; set; }
    public DateTime OpenedAt { get; set; }
    public OrderStatus Status { get; set; }

    // Concurrency token — rejects a stale update instead of silently overwriting it
    // when two cashiers edit the same order (see CONTEXT.md known trade-offs).
    public byte[] RowVersion { get; set; } = null!;

    public int TableId { get; set; }
    public virtual RestaurantTable Table { get; set; } = null!;

    // The cashier/admin who opened this order — not necessarily who checks it out
    // (see Payment.CashierUserId for that).
    public int OpenedByUserId { get; set; }
    public virtual User OpenedByUser { get; set; } = null!;

    public virtual Payment? Payment { get; set; }

    private readonly List<OrderItem> _orderItems = new();

    // OrderItem is a composition of Order: it cannot exist without its parent,
    // so it is exposed only through methods here, never a public setter.
    public virtual IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public OrderItem AddItem(MenuItem menuItem, int quantity)
    {
        var item = new OrderItem
        {
            Order = this,
            OrderId = OrderId,
            MenuItem = menuItem,
            MenuItemId = menuItem.MenuItemId,
            Quantity = quantity,
            UnitPrice = menuItem.Price,
            Status = OrderItemStatus.Pending
        };
        _orderItems.Add(item);
        return item;
    }

    public decimal Total => _orderItems.Sum(i => i.UnitPrice * i.Quantity);
}
