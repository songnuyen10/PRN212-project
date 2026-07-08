using System.Windows;
using RestaurantPOS.WpfApp.ViewModels;

namespace RestaurantPOS.WpfApp;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = (LoginViewModel)DataContext;
        var user = viewModel.TryLogin(TxtPassword.Password);
        if (user == null) return;

        SessionContext.CurrentUser = user;
        new MainWindow().Show();
        Close();
    }
}
