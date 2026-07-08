using System.Collections.ObjectModel;
using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using RestaurantPOS.WpfApp.MVVM;

namespace RestaurantPOS.WpfApp.ViewModels;

public class TableMapViewModel : ViewModelBase
{
    private readonly ITableService _tableService = new TableService();
    private readonly IOrderService _orderService = new OrderService();

    private ObservableCollection<RestaurantTable> _tables = new();
    public ObservableCollection<RestaurantTable> Tables
    {
        get => _tables;
        set => SetField(ref _tables, value);
    }

    public RelayCommand RefreshCommand { get; }

    public TableMapViewModel()
    {
        RefreshCommand = new RelayCommand(_ => Refresh());
        Refresh();
    }

    public void Refresh()
    {
        Tables = new ObservableCollection<RestaurantTable>(_tableService.GetTables());
    }

    // Free table → open a new Order; Occupied/AwaitingPayment → resume its open Order.
    public int OpenOrderForTable(RestaurantTable table)
    {
        var order = table.Status == TableStatus.Free
            ? _orderService.CreateOrder(table.TableId, SessionContext.CurrentUser!.UserId)
            : _orderService.GetOpenOrderByTable(table.TableId);
        return order?.OrderId ?? 0;
    }
}
