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
}
