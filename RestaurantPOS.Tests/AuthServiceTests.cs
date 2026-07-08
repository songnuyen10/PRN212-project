using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using Xunit;

namespace RestaurantPOS.Tests;

public class AuthServiceTests
{
    [Fact]
    public void Login_ReturnsUser_WhenPasswordIsCorrect()
    {
        var userRepository = new FakeUserRepository();
        var (hash, salt) = PasswordHasher.Hash("Correct@123");
        userRepository.Seed(new User { UserId = 1, Username = "cashier1", PasswordHash = hash, PasswordSalt = salt, FullName = "Cashier One", Role = UserRole.Cashier });
        var service = new AuthService(userRepository);

        var result = service.Login("cashier1", "Correct@123");

        Assert.NotNull(result);
        Assert.Equal(1, result!.UserId);
    }

    [Fact]
    public void Login_ReturnsNull_WhenPasswordIsWrong()
    {
        var userRepository = new FakeUserRepository();
        var (hash, salt) = PasswordHasher.Hash("Correct@123");
        userRepository.Seed(new User { UserId = 1, Username = "cashier1", PasswordHash = hash, PasswordSalt = salt, FullName = "Cashier One", Role = UserRole.Cashier });
        var service = new AuthService(userRepository);

        var result = service.Login("cashier1", "WrongPassword");

        Assert.Null(result);
    }

    [Fact]
    public void Login_ReturnsNull_WhenUsernameIsUnknown()
    {
        var service = new AuthService(new FakeUserRepository());

        var result = service.Login("nobody", "whatever");

        Assert.Null(result);
    }

    [Fact]
    public void CreateUser_StoresAHashAndSaltThatLoginCanLaterVerify()
    {
        var userRepository = new FakeUserRepository();
        var service = new AuthService(userRepository);

        service.CreateUser("newcashier", "Secret@1", "New Cashier", UserRole.Cashier);
        var result = service.Login("newcashier", "Secret@1");

        Assert.NotNull(result);
        Assert.Equal("New Cashier", result!.FullName);
    }
}
