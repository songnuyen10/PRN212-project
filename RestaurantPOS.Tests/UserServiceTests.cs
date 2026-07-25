using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using Xunit;

namespace RestaurantPOS.Tests;

public class UserServiceTests
{
    [Fact]
    public void UpdateScheduledHours_ReturnsFalse_WhenEndIsNotAfterStart()
    {
        var repository = new FakeUserRepository();
        repository.Seed(new User { UserId = 1, Username = "cashier1", FullName = "Cashier One" });
        var service = new UserService(repository);

        var result = service.UpdateScheduledHours(1, TimeSpan.FromHours(17), TimeSpan.FromHours(8));

        Assert.False(result);
    }

    [Fact]
    public void UpdateScheduledHours_Succeeds_WhenEndIsAfterStart()
    {
        var repository = new FakeUserRepository();
        repository.Seed(new User { UserId = 1, Username = "cashier1", FullName = "Cashier One" });
        var service = new UserService(repository);

        var result = service.UpdateScheduledHours(1, TimeSpan.FromHours(8), TimeSpan.FromHours(17));

        Assert.True(result);
    }
}
