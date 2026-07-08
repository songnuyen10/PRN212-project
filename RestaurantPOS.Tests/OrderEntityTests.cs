using RestaurantPOS.BusinessObjects;
using Xunit;

namespace RestaurantPOS.Tests;

public class OrderEntityTests
{
    [Fact]
    public void AddItem_IsTheOnlyWayToPopulateOrderItems()
    {
        var order = new Order { OrderId = 1 };
        var menuItem = new MenuItem { MenuItemId = 1, ItemName = "Spring Rolls", Price = 45_000 };

        order.AddItem(menuItem, quantity: 2);

        var item = Assert.Single(order.OrderItems);
        Assert.Same(order, item.Order);
        Assert.Equal(45_000, item.UnitPrice);
        Assert.Equal(OrderItemStatus.Pending, item.Status);
    }

    [Fact]
    public void Total_SumsUnitPriceTimesQuantityAcrossAllItems()
    {
        var order = new Order { OrderId = 1 };
        order.AddItem(new MenuItem { MenuItemId = 1, ItemName = "Spring Rolls", Price = 45_000 }, quantity: 2);
        order.AddItem(new MenuItem { MenuItemId = 2, ItemName = "Iced Tea", Price = 15_000 }, quantity: 3);

        Assert.Equal(45_000 * 2 + 15_000 * 3, order.Total);
    }
}
