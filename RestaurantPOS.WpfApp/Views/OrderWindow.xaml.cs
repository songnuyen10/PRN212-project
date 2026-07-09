using System.Windows;
using RestaurantPOS.WpfApp.ViewModels;

namespace RestaurantPOS.WpfApp.Views;

public partial class OrderWindow : Window
{
    public OrderWindow(int orderId)
    {
        InitializeComponent();
        DataContext = new OrderViewModel(orderId);
    }

    private void Checkout_Click(object sender, RoutedEventArgs e)
    {
        var button = (UIElement)sender;
        button.IsEnabled = false;
        try
        {
            var viewModel = (OrderViewModel)DataContext;
            var paymentWindow = new PaymentWindow(viewModel.OrderId) { Owner = this };
            if (paymentWindow.ShowDialog() == true)
            {
                Close();
            }
        }
        finally
        {
            button.IsEnabled = true;
        }
    }
}
