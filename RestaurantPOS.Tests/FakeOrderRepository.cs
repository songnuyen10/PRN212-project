using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Tests;

// In-memory fake — no DB, no mocking library, just enough of IOrderRepository to
// drive OrderService in isolation.
public class FakeOrderRepository : IOrderRepository
{
    private readonly Dictionary<int, Order> _orders = new();
    private int _nextOrderId = 1;
    public bool AddItemsToOrderWasCalled { get; private set; }
    public AddItemsResult ResultToReturn { get; set; } = AddItemsResult.Success;

    public void Seed(Order order) => _orders[order.OrderId] = order;

    public Order? CreateOrder(int tableId, int openedByUserId)
    {
        var order = new Order { OrderId = _nextOrderId++, TableId = tableId, OpenedByUserId = openedByUserId, Status = OrderStatus.Open, OpenedAt = DateTime.Now };
        _orders[order.OrderId] = order;
        return order;
    }

    public Order? GetOpenOrderByTable(int tableId) =>
        _orders.Values.FirstOrDefault(o => o.TableId == tableId && o.Status == OrderStatus.Open);

    public Order? GetOrderById(int orderId) => _orders.GetValueOrDefault(orderId);

    public AddItemsResult AddItemsToOrder(int orderId, IReadOnlyList<(int MenuItemId, int Quantity)> lines)
    {
        AddItemsToOrderWasCalled = true;
        return ResultToReturn;
    }

    public bool CancelOrder(int orderId, int cancelledByUserId, string reason)
    {
        if (!_orders.TryGetValue(orderId, out var order) || order.Status != OrderStatus.Open) return false;
        order.Status = OrderStatus.Cancelled;
        order.CancelledByUserId = cancelledByUserId;
        order.CancelReason = reason;
        return true;
    }

    public List<OrderItem> GetKitchenQueue() => throw new NotImplementedException();

    public bool UpdateOrderItemStatus(int orderItemId, OrderItemStatus status) => throw new NotImplementedException();
}
