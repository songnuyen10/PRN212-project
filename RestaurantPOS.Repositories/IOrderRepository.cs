using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Repositories;

public interface IOrderRepository
{
    Order? CreateOrder(int tableId, int openedByUserId);
    Order? GetOpenOrderByTable(int tableId);
    Order? GetOrderById(int orderId);
    bool AddItemToOrder(int orderId, int menuItemId, int quantity);
    List<OrderItem> GetKitchenQueue();
    bool UpdateOrderItemStatus(int orderItemId, OrderItemStatus status);
}
