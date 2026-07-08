using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using Xunit;

namespace RestaurantPOS.Tests;

public class DashboardServiceTests
{
    [Fact]
    public void GetDailyRevenue_GroupsAndSumsPaymentsByDate()
    {
        var repository = new FakePaymentRepository();
        var day1 = new DateTime(2026, 7, 1, 10, 0, 0);
        var day2 = new DateTime(2026, 7, 2, 12, 0, 0);
        repository.Seed(new Payment { AmountPaid = 45_000, PaidAt = day1 });
        repository.Seed(new Payment { AmountPaid = 55_000, PaidAt = day1.AddHours(2) });
        repository.Seed(new Payment { AmountPaid = 65_000, PaidAt = day2 });
        var service = new DashboardService(repository);

        var result = service.GetDailyRevenue(day1.Date, day2.Date.AddDays(1));

        Assert.Equal(2, result.Count);
        Assert.Equal(100_000, result[0].Revenue);
        Assert.Equal(65_000, result[1].Revenue);
    }

    [Fact]
    public void GetTopSellers_RanksByQuantitySoldDescending()
    {
        var menuItem1 = new MenuItem { MenuItemId = 1, ItemName = "Spring Rolls", Price = 45_000 };
        var menuItem2 = new MenuItem { MenuItemId = 2, ItemName = "Chicken Rice", Price = 55_000 };

        var order = new Order { OrderId = 1 };
        order.AddItem(menuItem1, 5);
        order.AddItem(menuItem2, 1);

        var repository = new FakePaymentRepository();
        repository.Seed(new Payment { Order = order, PaidAt = DateTime.Today });
        var service = new DashboardService(repository);

        var result = service.GetTopSellers(DateTime.Today, DateTime.Today.AddDays(1));

        Assert.Equal("Spring Rolls", result[0].ItemName);
        Assert.Equal(5, result[0].QuantitySold);
    }
}
