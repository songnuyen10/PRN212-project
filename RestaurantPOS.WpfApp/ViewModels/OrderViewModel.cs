using System.Collections.ObjectModel;
using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using RestaurantPOS.WpfApp.MVVM;

namespace RestaurantPOS.WpfApp.ViewModels;

public class OrderViewModel : ViewModelBase
{
    private readonly IOrderService _orderService = new OrderService();
    private readonly IMenuItemService _menuItemService = new MenuItemService();
    private readonly ITableService _tableService = new TableService();

    public int OrderId { get; }

    private Order? _currentOrder;
    public Order? CurrentOrder
    {
        get => _currentOrder;
        private set => SetField(ref _currentOrder, value);
    }

    public ObservableCollection<MenuItem> AvailableMenuItems { get; } = new();

    // Already persisted, visible to the kitchen — rendered muted so staff don't re-send.
    public ObservableCollection<OrderItem> SentItems { get; } = new();

    // Picked but not yet sent — still editable/removable.
    public ObservableCollection<CartLine> DraftLines { get; } = new();

    private MenuItem? _selectedMenuItem;
    public MenuItem? SelectedMenuItem
    {
        get => _selectedMenuItem;
        set => SetField(ref _selectedMenuItem, value);
    }

    private int _quantity = 1;
    public int Quantity
    {
        get => _quantity;
        set => SetField(ref _quantity, value);
    }

    public decimal SentTotal => CurrentOrder?.Total ?? 0m;
    public decimal DraftTotal => DraftLines.Sum(l => l.Subtotal);

    public RelayCommand AddToCartCommand { get; }
    public RelayCommand RemoveDraftLineCommand { get; }
    public RelayCommand SendToKitchenCommand { get; }
    public RelayCommand MarkAwaitingPaymentCommand { get; }

    public OrderViewModel(int orderId)
    {
        OrderId = orderId;

        AddToCartCommand = new RelayCommand(_ => AddToCart(), _ => SelectedMenuItem != null && Quantity > 0);
        RemoveDraftLineCommand = new RelayCommand(line => DraftLines.Remove((CartLine)line!));
        SendToKitchenCommand = new RelayCommand(_ => SendToKitchen(), _ => DraftLines.Count > 0);
        MarkAwaitingPaymentCommand = new RelayCommand(_ => MarkAwaitingPayment());

        Load();
    }

    private void Load()
    {
        CurrentOrder = _orderService.GetOrderById(OrderId);

        AvailableMenuItems.Clear();
        foreach (var item in _menuItemService.GetMenuItems().Where(m => m.IsAvailable))
        {
            AvailableMenuItems.Add(item);
        }

        SentItems.Clear();
        if (CurrentOrder != null)
        {
            foreach (var item in CurrentOrder.OrderItems)
            {
                SentItems.Add(item);
            }
        }

        OnPropertyChanged(nameof(SentTotal));
    }

    private void AddToCart()
    {
        var existing = DraftLines.FirstOrDefault(l => l.MenuItem.MenuItemId == SelectedMenuItem!.MenuItemId);
        if (existing != null)
        {
            existing.Quantity += Quantity;
        }
        else
        {
            DraftLines.Add(new CartLine { MenuItem = SelectedMenuItem!, Quantity = Quantity });
        }
        OnPropertyChanged(nameof(DraftTotal));
    }

    private void SendToKitchen()
    {
        foreach (var line in DraftLines)
        {
            _orderService.AddItemToOrder(OrderId, line.MenuItem.MenuItemId, line.Quantity);
        }
        DraftLines.Clear();
        Load();
        OnPropertyChanged(nameof(DraftTotal));
    }

    private void MarkAwaitingPayment()
    {
        if (CurrentOrder == null) return;
        _tableService.MarkAwaitingPayment(CurrentOrder.TableId);
    }
}
