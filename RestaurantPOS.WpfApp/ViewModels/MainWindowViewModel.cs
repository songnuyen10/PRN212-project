using RestaurantPOS.BusinessObjects;
using RestaurantPOS.WpfApp.MVVM;

namespace RestaurantPOS.WpfApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string WelcomeText { get; }

    // Role-scoped nav — Admin sees everything, Cashier runs the floor, Kitchen only sees the queue.
    public bool CanSeeTableMap { get; }
    public bool CanSeeShift { get; }
    public bool CanSeeKitchen { get; }
    public bool CanSeeMenu { get; }
    public bool CanSeeInventory { get; }
    public bool CanSeeDashboard { get; }

    public MainWindowViewModel()
    {
        var user = SessionContext.CurrentUser!;
        WelcomeText = $"Xin chào, {user.FullName} ({user.Role})";

        CanSeeTableMap = user.Role is UserRole.Admin or UserRole.Cashier;
        CanSeeShift = user.Role is UserRole.Admin or UserRole.Cashier;
        CanSeeKitchen = user.Role is UserRole.Admin or UserRole.KitchenStaff;
        CanSeeMenu = user.Role == UserRole.Admin;
        CanSeeInventory = user.Role == UserRole.Admin;
        CanSeeDashboard = user.Role == UserRole.Admin;
    }
}
