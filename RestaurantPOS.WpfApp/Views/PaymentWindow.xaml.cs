using System.Windows;
using RestaurantPOS.WpfApp.ViewModels;

namespace RestaurantPOS.WpfApp.Views;

public partial class PaymentWindow : Window
{
    public PaymentWindow(int orderId)
    {
        InitializeComponent();
        DataContext = new PaymentViewModel(orderId);
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        var button = (UIElement)sender;
        button.IsEnabled = false;
        try
        {
            ((PaymentViewModel)DataContext).Confirm();
        }
        finally
        {
            button.IsEnabled = true;
        }
    }

    private void Print_Click(object sender, RoutedEventArgs e)
    {
        ((PaymentViewModel)DataContext).PrintReceipt();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}
