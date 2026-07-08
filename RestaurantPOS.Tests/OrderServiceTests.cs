using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using Xunit;

namespace RestaurantPOS.Tests;

public class OrderServiceTests
{
    [Fact]
    public void AddItemToOrder_ReturnsFalse_WhenOrderIsNotOpen()
    {
        var orderRepository = new FakeOrderRepository();
        orderRepository.Seed(new Order { OrderId = 1, Status = OrderStatus.Paid });
        var service = new OrderService(orderRepository, new FakeRestaurantTableRepository());

        var result = service.AddItemToOrder(orderId: 1, menuItemId: 1, quantity: 1);

        Assert.False(result);
        Assert.False(orderRepository.AddItemToOrderWasCalled);
    }

    [Fact]
    public void AddItemToOrder_Succeeds_WhenOrderIsOpen()
    {
        var orderRepository = new FakeOrderRepository();
        orderRepository.Seed(new Order { OrderId = 1, Status = OrderStatus.Open });
        var service = new OrderService(orderRepository, new FakeRestaurantTableRepository());

        var result = service.AddItemToOrder(orderId: 1, menuItemId: 1, quantity: 1);

        Assert.True(result);
        Assert.True(orderRepository.AddItemToOrderWasCalled);
    }

    [Fact]
    public void CreateOrder_ReturnsOrder_WhenTableIsFree()
    {
        var tableRepository = new FakeRestaurantTableRepository();
        tableRepository.Seed(new RestaurantTable { TableId = 1, TableName = "T1", Capacity = 4, Status = TableStatus.Free });
        var service = new OrderService(new FakeOrderRepository(), tableRepository);

        var result = service.CreateOrder(tableId: 1, openedByUserId: 7);

        Assert.NotNull(result);
        Assert.Equal(1, result!.TableId);
        Assert.Equal(7, result.OpenedByUserId);
    }

    [Fact]
    public void CreateOrder_ReturnsNull_WhenTableIsOccupied()
    {
        var tableRepository = new FakeRestaurantTableRepository();
        tableRepository.Seed(new RestaurantTable { TableId = 1, TableName = "T1", Capacity = 4, Status = TableStatus.Occupied });
        var service = new OrderService(new FakeOrderRepository(), tableRepository);

        var result = service.CreateOrder(tableId: 1, openedByUserId: 7);

        Assert.Null(result);
    }

    [Fact]
    public void CreateOrder_ReturnsNull_WhenTableDoesNotExist()
    {
        var service = new OrderService(new FakeOrderRepository(), new FakeRestaurantTableRepository());

        var result = service.CreateOrder(tableId: 999, openedByUserId: 7);

        Assert.Null(result);
    }
}
