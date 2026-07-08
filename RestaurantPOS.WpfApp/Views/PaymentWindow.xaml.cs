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
        var viewModel = (PaymentViewModel)DataContext;
        if (viewModel.Confirm())
        {
            DialogResult = true;
        }
    }
}
