using Microsoft.EntityFrameworkCore;
using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.DataAccessObjects;

public class PaymentDAO
{
    // Eligibility (order must be Open) is validated by PaymentService before this is
    // called. This closes the order, frees the table, and deducts ingredient stock
    // for every item served — all as one SaveChanges call (one DB transaction), so a
    // payment can never be recorded without its inventory deduction (see CONTEXT.md).
    public static CheckoutResult CheckoutOrder(int orderId, int cashierUserId, PaymentMethod method, int? shiftId)
    {
        using var context = new AppDbContext();
        try
        {
            var order = context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.MenuItem)
                .ThenInclude(m => m.MenuItemIngredients)
                .ThenInclude(mi => mi.Ingredient)
                .FirstOrDefault(o => o.OrderId == orderId);
            if (order == null) return CheckoutResult.OrderNotOpen;

            // Check every recipe line has enough stock before mutating anything —
            // a checkout must never leave QuantityInStock negative.
            foreach (var orderItem in order.OrderItems)
            {
                foreach (var recipeLine in orderItem.MenuItem.MenuItemIngredients)
                {
                    if (recipeLine.Ingredient.QuantityInStock < recipeLine.QuantityRequired * orderItem.Quantity)
                    {
                        return CheckoutResult.InsufficientStock;
                    }
                }
            }

            foreach (var orderItem in order.OrderItems)
            {
                foreach (var recipeLine in orderItem.MenuItem.MenuItemIngredients)
                {
                    recipeLine.Ingredient.QuantityInStock -= recipeLine.QuantityRequired * orderItem.Quantity;
                }
            }

            context.Payments.Add(new Payment
            {
                OrderId = orderId,
                CashierUserId = cashierUserId,
                ShiftId = shiftId,
                AmountPaid = order.Total,
                Method = method,
                PaidAt = DateTime.Now
            });
            order.Status = OrderStatus.Paid;
            order.Table.Status = TableStatus.Free;

            context.SaveChanges();
            return CheckoutResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another cashier already modified this order — reject rather than overwrite.
            return CheckoutResult.Conflict;
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"{nameof(PaymentDAO)}.{nameof(CheckoutOrder)}", ex);
            return CheckoutResult.Error;
        }
    }

    // Cash-reconciliation source: every payment recorded against a given shift.
    public static List<Payment> GetPaymentsByShiftId(int shiftId)
    {
        using var context = new AppDbContext();
        return context.Payments
            .Where(p => p.ShiftId == shiftId)
            .ToList();
    }

    public static List<Payment> GetPaymentsBetween(DateTime from, DateTime to)
    {
        using var context = new AppDbContext();
        return context.Payments
            .Include(p => p.Order)
            .ThenInclude(o => o.OrderItems)
            .ThenInclude(i => i.MenuItem)
            .Where(p => p.PaidAt >= from && p.PaidAt <= to)
            .ToList();
    }
}
