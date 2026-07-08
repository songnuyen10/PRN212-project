using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService()
    {
        _userRepository = new UserRepository();
    }

    // Test seam only — production code always uses the parameterless constructor.
    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User? Login(string username, string password)
    {
        var user = _userRepository.GetUserByUsername(username);
        if (user == null) return null;
        return PasswordHasher.Verify(password, user.PasswordHash, user.PasswordSalt) ? user : null;
    }

    public bool CreateUser(string username, string password, string fullName, UserRole role)
    {
        var (hash, salt) = PasswordHasher.Hash(password);
        var user = new User
        {
            Username = username,
            PasswordHash = hash,
            PasswordSalt = salt,
            FullName = fullName,
            Role = role
        };
        return _userRepository.SaveUser(user);
    }
}
