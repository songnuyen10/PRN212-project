using System.Windows;
using RestaurantPOS.WpfApp.ViewModels;

namespace RestaurantPOS.WpfApp.Views;

public partial class KitchenWindow : Window
{
    public KitchenWindow()
    {
        InitializeComponent();
    }

    private void KitchenWindow_Closed(object? sender, EventArgs e)
    {
        ((KitchenViewModel)DataContext).StopPolling();
    }
}
