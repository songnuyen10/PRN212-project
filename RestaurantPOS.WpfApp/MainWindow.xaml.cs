using System.Windows;
using RestaurantPOS.WpfApp.ViewModels;
using RestaurantPOS.WpfApp.Views;

namespace RestaurantPOS.WpfApp;

public partial class MainWindow : Window
{
    private readonly Dictionary<Type, Window> _openWindows = new();
    private bool _shiftGateShown;

    public MainWindow()
    {
        InitializeComponent();
        Activated += (_, _) => ((MainWindowViewModel)DataContext).RefreshShift();
        ContentRendered += MainWindow_ContentRendered;
    }

    // Post-login shift gate. Runs off ContentRendered (not the LoginWindow path)
    // so a DB error here can never strand the user before MainWindow.Show()
    // returns — worst case the gate silently doesn't show.
    private void MainWindow_ContentRendered(object? sender, EventArgs e)
    {
        if (_shiftGateShown) return;
        _shiftGateShown = true;

        var viewModel = (MainWindowViewModel)DataContext;
        if (!viewModel.CanSeeShift) return;

        viewModel.RefreshShift();
        if (viewModel.HasOpenShift) return;

        var gate = new ShiftWindow(forceOpen: viewModel.RequiresShiftGate, showSkipButton: !viewModel.RequiresShiftGate) { Owner = this };
        gate.ShowDialog();
        viewModel.RefreshShift();
    }

    private void ShowSingle<T>() where T : Window, new()
    {
        if (_openWindows.TryGetValue(typeof(T), out var existing))
        {
            existing.Activate();
            return;
        }

        var window = new T();
        _openWindows[typeof(T)] = window;
        window.Closed += (_, _) => _openWindows.Remove(typeof(T));
        window.Show();
    }

    private void MnuTableMap_Click(object sender, RoutedEventArgs e) => ShowSingle<TableMapWindow>();

    private void MnuShift_Click(object sender, RoutedEventArgs e) => ShowSingle<ShiftWindow>();

    private void MnuKitchen_Click(object sender, RoutedEventArgs e) => ShowSingle<KitchenWindow>();

    private void MnuMenu_Click(object sender, RoutedEventArgs e) => ShowSingle<MenuManagementWindow>();

    private void MnuInventory_Click(object sender, RoutedEventArgs e) => ShowSingle<InventoryWindow>();

    private void MnuDashboard_Click(object sender, RoutedEventArgs e) => ShowSingle<DashboardWindow>();

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        var button = (UIElement)sender;
        button.IsEnabled = false;
        try
        {
            var viewModel = (MainWindowViewModel)DataContext;
            if (viewModel.HasOpenShift)
            {
                var confirm = MessageBox.Show(
                    "Bạn đang trong ca làm việc. Đăng xuất sẽ không đóng ca. Tiếp tục?",
                    "Xác nhận đăng xuất", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm != MessageBoxResult.Yes) return;
            }

            foreach (var window in _openWindows.Values.ToList())
            {
                window.Close();
            }
            SessionContext.CurrentUser = null;
            new LoginWindow().Show();
            Close();
        }
        finally
        {
            button.IsEnabled = true;
        }
    }
}
