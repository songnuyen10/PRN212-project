using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Services;

public interface IAuthService
{
    User? Login(string username, string password);
    bool CreateUser(string username, string password, string fullName, UserRole role);
}
