using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Tests;

public class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public void Seed(User user) => _users.Add(user);

    public User? GetUserByUsername(string username) => _users.FirstOrDefault(u => u.Username == username);

    public List<User> GetUsers() => _users;

    public bool SaveUser(User user)
    {
        _users.Add(user);
        return true;
    }

    public bool UpdateScheduledHours(int userId, TimeSpan? start, TimeSpan? end)
    {
        var user = _users.FirstOrDefault(u => u.UserId == userId);
        if (user == null) return false;
        user.ScheduledStartTime = start;
        user.ScheduledEndTime = end;
        return true;
    }
}
