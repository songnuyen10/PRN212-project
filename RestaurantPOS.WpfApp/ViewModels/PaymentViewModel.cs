using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Services;
using RestaurantPOS.WpfApp.MVVM;

namespace RestaurantPOS.WpfApp.ViewModels;

public class PaymentViewModel : ViewModelBase
{
    private readonly IOrderService _orderService = new OrderService();
    private readonly IPaymentService _paymentService = new PaymentService();

    public int OrderId { get; }
    public Order? CurrentOrder { get; }

    private PaymentMethod _selectedMethod = PaymentMethod.Cash;
    public PaymentMethod SelectedMethod
    {
        get => _selectedMethod;
        set => SetField(ref _selectedMethod, value);
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetField(ref _errorMessage, value);
    }

    public PaymentViewModel(int orderId)
    {
        OrderId = orderId;
        CurrentOrder = _orderService.GetOrderById(orderId);
    }

    public bool Confirm()
    {
        var cashierUserId = SessionContext.CurrentUser!.UserId;
        var success = _paymentService.Checkout(OrderId, cashierUserId, SelectedMethod);
        ErrorMessage = success ? string.Empty : "Thanh toán thất bại — đơn hàng có thể đã bị thay đổi bởi người khác.";
        return success;
    }
}
