using Microsoft.EntityFrameworkCore;
using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.DataAccessObjects;

public class OrderDAO
{
    // OrderService's Free-table check is only a fast-path hint — two staff can pass it
    // at nearly the same time. The real guard is the conditional UPDATE below: it only
    // occupies the table if it is still Free, atomically, so a losing concurrent call
    // gets 0 rows affected instead of creating a second Order on the same table.
    public static Order? CreateOrder(int tableId, int openedByUserId)
    {
        using var context = new AppDbContext();
        using var transaction = context.Database.BeginTransaction();
        try
        {
            int rowsAffected = context.RestaurantTables
                .Where(t => t.TableId == tableId && t.Status == TableStatus.Free)
                .ExecuteUpdate(s => s.SetProperty(t => t.Status, TableStatus.Occupied));
            if (rowsAffected == 0) return null; // table missing, or lost the race to another order

            var order = new Order
            {
                TableId = tableId,
                OpenedByUserId = openedByUserId,
                OpenedAt = DateTime.Now,
                Status = OrderStatus.Open
            };
            context.Orders.Add(order);
            context.SaveChanges();
            transaction.Commit();
            return order;
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"{nameof(OrderDAO)}.{nameof(CreateOrder)}", ex);
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
            AppLogger.LogError($"{nameof(OrderDAO)}.{nameof(AddItemToOrder)}", ex);
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
            AppLogger.LogError($"{nameof(OrderDAO)}.{nameof(UpdateOrderItemStatus)}", ex);
            return false;
        }
    }
}
