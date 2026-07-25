using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRestaurantTableRepository _tableRepository;

    public OrderService()
    {
        _orderRepository = new OrderRepository();
        _tableRepository = new RestaurantTableRepository();
    }

    // Test seam only — production code always uses the parameterless constructor.
    public OrderService(IOrderRepository orderRepository, IRestaurantTableRepository tableRepository)
    {
        _orderRepository = orderRepository;
        _tableRepository = tableRepository;
    }

    public Order? CreateOrder(int tableId, int openedByUserId)
    {
        // Business rule: an order can only be opened on a free table.
        var table = _tableRepository.GetTableById(tableId);
        if (table == null || table.Status != TableStatus.Free) return null;

        return _orderRepository.CreateOrder(tableId, openedByUserId);
    }

    public Order? GetOpenOrderByTable(int tableId) => _orderRepository.GetOpenOrderByTable(tableId);

    public Order? GetOrderById(int orderId) => _orderRepository.GetOrderById(orderId);

    public AddItemsResult AddItemsToOrder(int orderId, IReadOnlyList<(int MenuItemId, int Quantity)> lines)
    {
        if (lines.Any(l => l.Quantity <= 0)) return AddItemsResult.Error;

        // Business rule: items can only be added to a still-open order.
        var order = _orderRepository.GetOrderById(orderId);
        if (order == null || order.Status != OrderStatus.Open) return AddItemsResult.Error;

        return _orderRepository.AddItemsToOrder(orderId, lines);
    }

    public bool CancelOrder(int orderId, int cancelledByUserId, string reason)
    {
        // Business rule: a cancel reason is required, and only an Open order can be cancelled.
        if (string.IsNullOrWhiteSpace(reason)) return false;

        var order = _orderRepository.GetOrderById(orderId);
        if (order == null || order.Status != OrderStatus.Open) return false;

        return _orderRepository.CancelOrder(orderId, cancelledByUserId, reason);
    }

    public List<OrderItem> GetKitchenQueue() => _orderRepository.GetKitchenQueue();

    public bool UpdateOrderItemStatus(int orderItemId, OrderItemStatus status) =>
        _orderRepository.UpdateOrderItemStatus(orderItemId, status);
}
