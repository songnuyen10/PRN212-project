using System.Windows;
using RestaurantPOS.WpfApp.Views;

namespace RestaurantPOS.WpfApp;

public partial class MainWindow : Window
{
    private readonly Dictionary<Type, Window> _openWindows = new();

    public MainWindow()
    {
        InitializeComponent();
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
}
