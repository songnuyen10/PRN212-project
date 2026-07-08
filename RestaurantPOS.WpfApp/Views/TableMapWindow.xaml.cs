using System.Windows;
using System.Windows.Input;
using RestaurantPOS.BusinessObjects;
using RestaurantPOS.WpfApp.ViewModels;

namespace RestaurantPOS.WpfApp.Views;

public partial class TableMapWindow : Window
{
    public TableMapWindow()
    {
        InitializeComponent();
    }

    private void Tile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount != 2) return;

        var table = (RestaurantTable)((FrameworkElement)sender).Tag;
        var viewModel = (TableMapViewModel)DataContext;
        var orderId = viewModel.OpenOrderForTable(table);
        if (orderId == 0) return;

        var orderWindow = new OrderWindow(orderId) { Owner = this };
        orderWindow.Closed += (_, _) => viewModel.Refresh();
        orderWindow.ShowDialog();
    }
}
