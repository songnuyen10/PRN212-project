using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Services;

public interface IOrderService
{
    Order? CreateOrder(int tableId, int openedByUserId);
    Order? GetOpenOrderByTable(int tableId);
    Order? GetOrderById(int orderId);
    AddItemsResult AddItemsToOrder(int orderId, IReadOnlyList<(int MenuItemId, int Quantity)> lines);
    bool CancelOrder(int orderId, int cancelledByUserId, string reason);
    List<OrderItem> GetKitchenQueue();
    bool UpdateOrderItemStatus(int orderItemId, OrderItemStatus status);
}
