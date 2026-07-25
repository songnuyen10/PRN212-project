using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using Xunit;

namespace RestaurantPOS.Tests;

public class OrderServiceTests
{
    [Fact]
    public void AddItemsToOrder_ReturnsError_WhenOrderIsNotOpen()
    {
        var orderRepository = new FakeOrderRepository();
        orderRepository.Seed(new Order { OrderId = 1, Status = OrderStatus.Paid });
        var service = new OrderService(orderRepository, new FakeRestaurantTableRepository());

        var result = service.AddItemsToOrder(orderId: 1, lines: [(1, 1)]);

        Assert.Equal(AddItemsResult.Error, result);
        Assert.False(orderRepository.AddItemsToOrderWasCalled);
    }

    [Fact]
    public void AddItemsToOrder_Succeeds_WhenOrderIsOpen()
    {
        var orderRepository = new FakeOrderRepository();
        orderRepository.Seed(new Order { OrderId = 1, Status = OrderStatus.Open });
        var service = new OrderService(orderRepository, new FakeRestaurantTableRepository());

        var result = service.AddItemsToOrder(orderId: 1, lines: [(1, 1)]);

        Assert.Equal(AddItemsResult.Success, result);
        Assert.True(orderRepository.AddItemsToOrderWasCalled);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItemsToOrder_ReturnsError_WhenAnyQuantityIsNotPositive(int quantity)
    {
        var orderRepository = new FakeOrderRepository();
        orderRepository.Seed(new Order { OrderId = 1, Status = OrderStatus.Open });
        var service = new OrderService(orderRepository, new FakeRestaurantTableRepository());

        var result = service.AddItemsToOrder(orderId: 1, lines: [(1, quantity)]);

        Assert.Equal(AddItemsResult.Error, result);
        Assert.False(orderRepository.AddItemsToOrderWasCalled);
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

    [Fact]
    public void CancelOrder_ReturnsFalse_WhenOrderIsNotOpen()
    {
        var orderRepository = new FakeOrderRepository();
        orderRepository.Seed(new Order { OrderId = 1, Status = OrderStatus.Paid });
        var service = new OrderService(orderRepository, new FakeRestaurantTableRepository());

        var result = service.CancelOrder(orderId: 1, cancelledByUserId: 1, reason: "Nhầm bàn");

        Assert.False(result);
    }

    [Fact]
    public void CancelOrder_ReturnsFalse_WhenReasonIsBlank()
    {
        var orderRepository = new FakeOrderRepository();
        orderRepository.Seed(new Order { OrderId = 1, Status = OrderStatus.Open });
        var service = new OrderService(orderRepository, new FakeRestaurantTableRepository());

        var result = service.CancelOrder(orderId: 1, cancelledByUserId: 1, reason: "  ");

        Assert.False(result);
    }

    [Fact]
    public void CancelOrder_Succeeds_WhenOrderIsOpenAndReasonProvided()
    {
        var orderRepository = new FakeOrderRepository();
        orderRepository.Seed(new Order { OrderId = 1, Status = OrderStatus.Open });
        var service = new OrderService(orderRepository, new FakeRestaurantTableRepository());

        var result = service.CancelOrder(orderId: 1, cancelledByUserId: 1, reason: "Nhầm bàn");

        Assert.True(result);
    }
}
