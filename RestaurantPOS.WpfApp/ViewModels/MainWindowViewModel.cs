using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using RestaurantPOS.WpfApp.MVVM;

namespace RestaurantPOS.WpfApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IShiftService _shiftService = new ShiftService();
    private readonly int _userId;

    public string WelcomeText { get; }

    // Role-scoped nav — Admin sees everything, Cashier runs the floor, Kitchen only sees the queue.
    public bool CanSeeTableMap { get; }
    public bool CanSeeShift { get; }
    public bool RequiresShiftGate { get; }
    public bool CanSeeKitchen { get; }
    public bool CanSeeMenu { get; }
    public bool CanSeeInventory { get; }
    public bool CanSeeDashboard { get; }

    private string _shiftStatusText = string.Empty;
    public string ShiftStatusText
    {
        get => _shiftStatusText;
        private set => SetField(ref _shiftStatusText, value);
    }

    private bool _hasOpenShift;
    public bool HasOpenShift
    {
        get => _hasOpenShift;
        private set => SetField(ref _hasOpenShift, value);
    }

    public MainWindowViewModel()
    {
        var user = SessionContext.CurrentUser!;
        _userId = user.UserId;
        WelcomeText = $"Xin chào, {user.FullName} ({user.Role})";

        CanSeeTableMap = user.Role is UserRole.Admin or UserRole.Cashier;
        CanSeeShift = user.Role is UserRole.Admin or UserRole.Cashier;
        // Cashier can't skip the post-login shift gate (cash payments need an open
        // shift); Admin sees it too but can dismiss it.
        RequiresShiftGate = user.Role == UserRole.Cashier;
        CanSeeKitchen = user.Role is UserRole.Admin or UserRole.KitchenStaff;
        CanSeeMenu = user.Role == UserRole.Admin;
        CanSeeInventory = user.Role == UserRole.Admin;
        CanSeeDashboard = user.Role == UserRole.Admin;
    }

    // Called on MainWindow.Activated — also covers the initial load, so the ctor
    // doesn't need to hit the DB itself.
    public void RefreshShift()
    {
        var shift = _shiftService.GetOpenShift(_userId);
        HasOpenShift = shift != null;
        ShiftStatusText = shift == null
            ? "Chưa mở ca — mở ở menu Ca làm việc"
            : $"Ca: mở lúc {shift.OpenedAt:HH:mm}";
    }
}
