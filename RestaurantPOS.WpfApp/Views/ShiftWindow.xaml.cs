using System.ComponentModel;
using System.Windows;
using RestaurantPOS.WpfApp.ViewModels;

namespace RestaurantPOS.WpfApp.Views;

public partial class ShiftWindow : Window
{
    private bool _isGate;
    private bool _forceOpen;
    private bool _exiting;

    public ShiftWindow()
    {
        InitializeComponent();
        Closing += ShiftWindow_Closing;
        Closed += (_, _) => ((ShiftViewModel)DataContext).StopTimer();
        // Refresh reconciliation numbers whenever this window regains focus — a
        // payment taken elsewhere (e.g. PaymentWindow) while this stays open would
        // otherwise leave stale "expected cash" on screen for the cashier to close against.
        Activated += (_, _) => ((ShiftViewModel)DataContext).Load();
    }

    // Used when MainWindow opens this as a post-login gate — forceOpen blocks the
    // Cashier from closing without an open shift; showSkipButton lets Admin dismiss.
    public ShiftWindow(bool forceOpen, bool showSkipButton) : this()
    {
        _isGate = true;
        _forceOpen = forceOpen;
        BtnSkip.Visibility = showSkipButton ? Visibility.Visible : Visibility.Collapsed;
        BtnExit.Visibility = forceOpen ? Visibility.Visible : Visibility.Collapsed;

        var viewModel = (ShiftViewModel)DataContext;
        viewModel.PropertyChanged += (_, args) =>
        {
            if (_isGate && args.PropertyName == nameof(ShiftViewModel.HasOpenShift) && viewModel.HasOpenShift)
            {
                Close();
            }
        };
    }

    private void ShiftWindow_Closing(object? sender, CancelEventArgs e)
    {
        if (_exiting) return;
        var viewModel = (ShiftViewModel)DataContext;
        if (_forceOpen && !viewModel.HasOpenShift)
        {
            e.Cancel = true;
        }
    }

    private void BtnSkip_Click(object sender, RoutedEventArgs e) => Close();

    private void BtnExit_Click(object sender, RoutedEventArgs e)
    {
        _exiting = true;
        Application.Current.Shutdown();
    }
}
