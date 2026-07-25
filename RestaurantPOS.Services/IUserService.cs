using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.Services;

public interface IUserService
{
    List<User> GetUsers();
    bool UpdateScheduledHours(int userId, TimeSpan? start, TimeSpan? end);
}
