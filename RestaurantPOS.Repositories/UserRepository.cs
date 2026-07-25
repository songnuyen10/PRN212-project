using RestaurantPOS.BusinessObjects;
using RestaurantPOS.DataAccessObjects;

namespace RestaurantPOS.Repositories;

public class UserRepository : IUserRepository
{
    public User? GetUserByUsername(string username) => UserDAO.GetUserByUsername(username);

    public List<User> GetUsers() => UserDAO.GetUsers();

    public bool SaveUser(User user) => UserDAO.SaveUser(user);

    public bool UpdateScheduledHours(int userId, TimeSpan? start, TimeSpan? end) =>
        UserDAO.UpdateScheduledHours(userId, start, end);
}
