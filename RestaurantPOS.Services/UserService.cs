using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService()
    {
        _userRepository = new UserRepository();
    }

    // Test seam only — production code always uses the parameterless constructor.
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public List<User> GetUsers() => _userRepository.GetUsers();

    public bool UpdateScheduledHours(int userId, TimeSpan? start, TimeSpan? end)
    {
        if (start.HasValue && end.HasValue && end <= start) return false;
        return _userRepository.UpdateScheduledHours(userId, start, end);
    }
}
