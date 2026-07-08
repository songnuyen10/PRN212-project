using System.Windows;
using RestaurantPOS.WpfApp.Views;

namespace RestaurantPOS.WpfApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void MnuTableMap_Click(object sender, RoutedEventArgs e) => new TableMapWindow().Show();

    private void MnuShift_Click(object sender, RoutedEventArgs e) => new ShiftWindow().Show();

    private void MnuKitchen_Click(object sender, RoutedEventArgs e) => new KitchenWindow().Show();

    private void MnuMenu_Click(object sender, RoutedEventArgs e) => new MenuManagementWindow().Show();

    private void MnuInventory_Click(object sender, RoutedEventArgs e) => new InventoryWindow().Show();

    private void MnuDashboard_Click(object sender, RoutedEventArgs e) => new DashboardWindow().Show();
}
