using Microsoft.EntityFrameworkCore;
using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.DataAccessObjects;

public class OrderDAO
{
    // Eligibility (table must be Free) is validated by OrderService before this is
    // called — this is a mechanical insert, not a decision point.
    public static Order? CreateOrder(int tableId, int openedByUserId)
    {
        using var context = new AppDbContext();
        try
        {
            var table = context.RestaurantTables.FirstOrDefault(t => t.TableId == tableId);
            if (table == null) return null;

            var order = new Order
            {
                TableId = tableId,
                OpenedByUserId = openedByUserId,
                OpenedAt = DateTime.Now,
                Status = OrderStatus.Open
            };
            context.Orders.Add(order);
            table.Status = TableStatus.Occupied;
            context.SaveChanges();
            return order;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            return null;
        }
    }

    public static Order? GetOpenOrderByTable(int tableId)
    {
        using var context = new AppDbContext();
        return context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.MenuItem)
            .FirstOrDefault(o => o.TableId == tableId && o.Status == OrderStatus.Open);
    }

    public static Order? GetOrderById(int orderId)
    {
        using var context = new AppDbContext();
        return context.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.MenuItem)
            .FirstOrDefault(o => o.OrderId == orderId);
    }

    public static bool AddItemToOrder(int orderId, int menuItemId, int quantity)
    {
        using var context = new AppDbContext();
        try
        {
            var order = context.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.OrderId == orderId);
            var menuItem = context.MenuItems.FirstOrDefault(m => m.MenuItemId == menuItemId);
            if (order == null || menuItem == null) return false;

            order.AddItem(menuItem, quantity);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            return false;
        }
    }

    // Every OrderItem not yet Done, across all open orders — the kitchen queue.
    public static List<OrderItem> GetKitchenQueue()
    {
        using var context = new AppDbContext();
        return context.OrderItems
            .Include(i => i.MenuItem)
            .Include(i => i.Order)
            .ThenInclude(o => o.Table)
            .Where(i => i.Status != OrderItemStatus.Done && i.Order.Status == OrderStatus.Open)
            .OrderBy(i => i.Order.OpenedAt)
            .ToList();
    }

    public static bool UpdateOrderItemStatus(int orderItemId, OrderItemStatus status)
    {
        using var context = new AppDbContext();
        try
        {
            var item = context.OrderItems.FirstOrDefault(i => i.OrderItemId == orderItemId);
            if (item == null) return false;
            item.Status = status;
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            return false;
        }
    }
}
