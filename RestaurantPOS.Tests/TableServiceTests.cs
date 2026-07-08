using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using Xunit;

namespace RestaurantPOS.Tests;

public class TableServiceTests
{
    [Fact]
    public void MarkAwaitingPayment_ReturnsFalse_WhenTableHasNoOpenOrder()
    {
        var service = new TableService(new FakeRestaurantTableRepository(), new FakeOrderRepository());

        var result = service.MarkAwaitingPayment(tableId: 1);

        Assert.False(result);
    }

    [Fact]
    public void MarkAwaitingPayment_Succeeds_WhenTableHasAnOpenOrder()
    {
        var tableRepository = new FakeRestaurantTableRepository();
        tableRepository.Seed(new RestaurantTable { TableId = 1, TableName = "T1", Capacity = 4, Status = TableStatus.Occupied });
        var orderRepository = new FakeOrderRepository();
        orderRepository.Seed(new Order { OrderId = 1, TableId = 1, Status = OrderStatus.Open });
        var service = new TableService(tableRepository, orderRepository);

        var result = service.MarkAwaitingPayment(tableId: 1);

        Assert.True(result);
        Assert.Equal(TableStatus.AwaitingPayment, tableRepository.GetTableById(1)!.Status);
    }
}
