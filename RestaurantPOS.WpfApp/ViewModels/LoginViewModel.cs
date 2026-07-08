using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using RestaurantPOS.WpfApp.MVVM;

namespace RestaurantPOS.WpfApp.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService = new AuthService();

    private string _username = string.Empty;
    public string Username
    {
        get => _username;
        set => SetField(ref _username, value);
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetField(ref _errorMessage, value);
    }

    // Set from LoginWindow's code-behind, which reads the PasswordBox directly —
    // PasswordBox.Password cannot be data-bound for security reasons.
    public User? TryLogin(string password)
    {
        var user = _authService.Login(Username, password);
        ErrorMessage = user == null ? "Sai tên đăng nhập hoặc mật khẩu." : string.Empty;
        return user;
    }
}
