using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Services;

public interface IUserService
{
    List<User> GetUsers();
}
