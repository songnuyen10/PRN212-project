using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Repositories;

public interface IUserRepository
{
    User? GetUserByUsername(string username);
    List<User> GetUsers();
    bool SaveUser(User user);
}
