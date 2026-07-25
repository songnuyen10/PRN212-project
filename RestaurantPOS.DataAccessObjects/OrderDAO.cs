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
            .Include(o => o.Payment)
            .FirstOrDefault(o => o.OrderId == orderId);
    }

    // Sends a whole cart to the kitchen as one SaveChanges (was previously one
    // call per line, so a mid-batch failure could leave a partial cart persisted).
    // Also checks stock for (items already on the order + this batch) before
    // writing anything — the earliest point in the flow the ingredient count is
    // known, instead of leaving the cashier stuck at checkout with a full cart.
    // ponytail: per-order guard only, doesn't see stock reserved by OTHER open
    // orders — same accepted limit as the checkout guard in PaymentDAO below.
    public static AddItemsResult AddItemsToOrder(int orderId, IReadOnlyList<(int MenuItemId, int Quantity)> lines)
    {
        using var context = new AppDbContext();
        try
        {
            var order = context.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.OrderId == orderId);
            if (order == null || order.Status != OrderStatus.Open) return AddItemsResult.Error;

            var menuItemIds = order.OrderItems.Select(i => i.MenuItemId)
                .Union(lines.Select(l => l.MenuItemId))
                .Distinct()
                .ToList();

            var menuItems = context.MenuItems
                .Include(m => m.MenuItemIngredients)
                .ThenInclude(mi => mi.Ingredient)
                .Where(m => menuItemIds.Contains(m.MenuItemId))
                .ToDictionary(m => m.MenuItemId);

            if (lines.Any(l => !menuItems.ContainsKey(l.MenuItemId))) return AddItemsResult.Error;

            var ingredientsById = menuItems.Values
                .SelectMany(m => m.MenuItemIngredients)
                .Select(mi => mi.Ingredient)
                .DistinctBy(i => i.IngredientId)
                .ToDictionary(i => i.IngredientId);

            var required = new Dictionary<int, decimal>();
            void Accumulate(int menuItemId, int quantity)
            {
                foreach (var recipeLine in menuItems[menuItemId].MenuItemIngredients)
                {
                    required[recipeLine.IngredientId] = required.GetValueOrDefault(recipeLine.IngredientId) + recipeLine.QuantityRequired * quantity;
                }
            }
            foreach (var item in order.OrderItems) Accumulate(item.MenuItemId, item.Quantity);
            foreach (var line in lines) Accumulate(line.MenuItemId, line.Quantity);

            foreach (var (ingredientId, quantityNeeded) in required)
            {
                if (ingredientsById[ingredientId].QuantityInStock < quantityNeeded) return AddItemsResult.InsufficientStock;
            }

            foreach (var line in lines)
            {
                order.AddItem(menuItems[line.MenuItemId], line.Quantity);
            }
            context.SaveChanges();
            return AddItemsResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            return AddItemsResult.Error;
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"{nameof(OrderDAO)}.{nameof(AddItemsToOrder)}", ex);
            return AddItemsResult.Error;
        }
    }

    // Frees a mis-opened/no-longer-wanted table. Only an Open order can be
    // cancelled — Paid stays immutable (see PaymentDAO guard below).
    public static bool CancelOrder(int orderId, int cancelledByUserId, string reason)
    {
        using var context = new AppDbContext();
        try
        {
            var order = context.Orders.Include(o => o.Table).FirstOrDefault(o => o.OrderId == orderId);
            if (order == null || order.Status != OrderStatus.Open) return false;

            order.Status = OrderStatus.Cancelled;
            order.CancelledByUserId = cancelledByUserId;
            order.CancelReason = reason;
            order.Table.Status = TableStatus.Free;

            context.SaveChanges();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"{nameof(OrderDAO)}.{nameof(CancelOrder)}", ex);
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
