using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;
using RestaurantPOS.DataAccessObjects;

namespace RestaurantPOS.WpfApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    // Last line of defense — any uncaught exception (DB hiccup on a read path,
    // a null session, ...) would otherwise crash the whole process with no trace.
    private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        AppLogger.LogError("UnhandledException", e.Exception);
        MessageBox.Show("Đã xảy ra lỗi không mong muốn. Vui lòng thử lại.", "Lỗi",
            MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}

