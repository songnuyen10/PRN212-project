using System.Collections.ObjectModel;
using System.Windows;
using RestaurantPOS.BusinessObjects;
using RestaurantPOS.DataAccessObjects;
using RestaurantPOS.Services;
using RestaurantPOS.WpfApp.MVVM;

namespace RestaurantPOS.WpfApp.ViewModels;

public class KitchenViewModel : ViewModelBase
{
    private readonly IOrderService _orderService = new OrderService();
    private readonly CancellationTokenSource _cts = new();

    public ObservableCollection<OrderItem> Queue { get; } = new();

    public RelayCommand MarkInProgressCommand { get; }
    public RelayCommand MarkDoneCommand { get; }

    public KitchenViewModel()
    {
        MarkInProgressCommand = new RelayCommand(item => SetStatus((OrderItem)item!, OrderItemStatus.InProgress));
        MarkDoneCommand = new RelayCommand(item => SetStatus((OrderItem)item!, OrderItemStatus.Done));

        _ = PollLoopAsync(_cts.Token);
    }

    // Background polling every 10s (not a DispatcherTimer) so querying the DB never
    // blocks the UI thread; results are marshalled back via the UI Dispatcher — see
    // CONTEXT.md's documented kitchen-display design.
    private async Task PollLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                // Runs the synchronous EF query on a thread-pool thread — without this,
                // the query executes on the UI thread every cycle (it's the code before
                // the first genuinely awaited yield in the loop).
                var items = await Task.Run(() => _orderService.GetKitchenQueue(), token).ConfigureAwait(false);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Queue.Clear();
                    foreach (var item in items) Queue.Add(item);
                });
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // A transient DB hiccup must not kill polling forever — log and retry next cycle.
                AppLogger.LogError($"{nameof(KitchenViewModel)}.{nameof(PollLoopAsync)}", ex);
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(10), token).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private void SetStatus(OrderItem item, OrderItemStatus status)
    {
        if (!_orderService.UpdateOrderItemStatus(item.OrderItemId, status))
        {
            MessageBox.Show("Không thể cập nhật trạng thái món — vui lòng thử lại.", "Lỗi",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        item.Status = status;
        if (status == OrderItemStatus.Done)
        {
            Queue.Remove(item);
        }
    }

    public void StopPolling() => _cts.Cancel();
}
