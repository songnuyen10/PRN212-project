using RestaurantPOS.BusinessObjects;
using RestaurantPOS.DataAccessObjects;

namespace RestaurantPOS.Repositories;

public class OrderRepository : IOrderRepository
{
    public Order? CreateOrder(int tableId, int openedByUserId) => OrderDAO.CreateOrder(tableId, openedByUserId);

    public Order? GetOpenOrderByTable(int tableId) => OrderDAO.GetOpenOrderByTable(tableId);

    public Order? GetOrderById(int orderId) => OrderDAO.GetOrderById(orderId);

    public AddItemsResult AddItemsToOrder(int orderId, IReadOnlyList<(int MenuItemId, int Quantity)> lines) => OrderDAO.AddItemsToOrder(orderId, lines);

    public bool CancelOrder(int orderId, int cancelledByUserId, string reason) => OrderDAO.CancelOrder(orderId, cancelledByUserId, reason);

    public List<OrderItem> GetKitchenQueue() => OrderDAO.GetKitchenQueue();

    public bool UpdateOrderItemStatus(int orderItemId, OrderItemStatus status) => OrderDAO.UpdateOrderItemStatus(orderItemId, status);
}
