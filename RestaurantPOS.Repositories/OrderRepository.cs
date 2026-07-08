using RestaurantPOS.BusinessObjects;
using RestaurantPOS.DataAccessObjects;

namespace RestaurantPOS.Repositories;

public class OrderRepository : IOrderRepository
{
    public Order? CreateOrder(int tableId, int openedByUserId) => OrderDAO.CreateOrder(tableId, openedByUserId);

    public Order? GetOpenOrderByTable(int tableId) => OrderDAO.GetOpenOrderByTable(tableId);

    public Order? GetOrderById(int orderId) => OrderDAO.GetOrderById(orderId);

    public bool AddItemToOrder(int orderId, int menuItemId, int quantity) => OrderDAO.AddItemToOrder(orderId, menuItemId, quantity);

    public List<OrderItem> GetKitchenQueue() => OrderDAO.GetKitchenQueue();

    public bool UpdateOrderItemStatus(int orderItemId, OrderItemStatus status) => OrderDAO.UpdateOrderItemStatus(orderItemId, status);
}
