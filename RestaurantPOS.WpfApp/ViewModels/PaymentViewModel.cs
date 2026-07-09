using System.Diagnostics;
using RestaurantPOS.BusinessObjects;
using RestaurantPOS.DataAccessObjects;
using RestaurantPOS.Services;
using RestaurantPOS.WpfApp.MVVM;
using RestaurantPOS.WpfApp.Reports;

namespace RestaurantPOS.WpfApp.ViewModels;

public class PaymentViewModel : ViewModelBase
{
    private readonly IOrderService _orderService = new OrderService();
    private readonly IPaymentService _paymentService = new PaymentService();

    public int OrderId { get; }
    public Order? CurrentOrder { get; }

    // No open shift doesn't block checkout (see PaymentService.Checkout), but the
    // cashier should know this payment won't land in any shift's cash reconciliation.
    public bool HasNoOpenShift { get; }

    private PaymentMethod _selectedMethod = PaymentMethod.Cash;
    public PaymentMethod SelectedMethod
    {
        get => _selectedMethod;
        set => SetField(ref _selectedMethod, value);
    }

    private decimal _amountTendered;
    public decimal AmountTendered
    {
        get => _amountTendered;
        set
        {
            SetField(ref _amountTendered, value);
            OnPropertyChanged(nameof(ChangeDue));
        }
    }

    public decimal ChangeDue => AmountTendered - (CurrentOrder?.Total ?? 0m);

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetField(ref _errorMessage, value);
    }

    private bool _isPaid;
    public bool IsPaid
    {
        get => _isPaid;
        set => SetField(ref _isPaid, value);
    }

    private string? _receiptPdfPath;

    public PaymentViewModel(int orderId)
    {
        OrderId = orderId;
        CurrentOrder = _orderService.GetOrderById(orderId);
        HasNoOpenShift = new ShiftService().GetOpenShift(SessionContext.CurrentUser!.UserId) == null;
    }

    public bool Confirm()
    {
        if (SelectedMethod == PaymentMethod.Cash && AmountTendered < (CurrentOrder?.Total ?? 0m))
        {
            ErrorMessage = "Tiền khách đưa không đủ.";
            return false;
        }

        var cashierUserId = SessionContext.CurrentUser!.UserId;
        var result = _paymentService.Checkout(OrderId, cashierUserId, SelectedMethod);
        ErrorMessage = result switch
        {
            CheckoutResult.Success => string.Empty,
            CheckoutResult.InsufficientStock => "Không đủ nguyên liệu để hoàn tất đơn hàng này.",
            CheckoutResult.Conflict => "Thanh toán thất bại — đơn hàng đã bị thay đổi bởi người khác.",
            CheckoutResult.OrderNotOpen => "Đơn hàng không còn ở trạng thái có thể thanh toán.",
            _ => "Thanh toán thất bại — vui lòng thử lại."
        };

        var success = result == CheckoutResult.Success;
        if (success)
        {
            // Payment already committed at this point — a receipt failure must not
            // block the success state or crash the app on the happy path.
            try
            {
                var paidOrder = _orderService.GetOrderById(OrderId);
                if (paidOrder != null)
                {
                    _receiptPdfPath = ReceiptBuilder.BuildPdf(paidOrder);
                }
            }
            catch (Exception ex)
            {
                _receiptPdfPath = null;
                AppLogger.LogError($"{nameof(PaymentViewModel)}.{nameof(Confirm)}", ex);
                ErrorMessage = "Thanh toán thành công nhưng không tạo được hóa đơn.";
            }
            IsPaid = true;
        }
        return success;
    }

    // No real printer to target here — "printing" opens the generated PDF with
    // the OS default viewer, which can print it. No PDF viewer registered (or the
    // temp file gone) must not crash the app, same as a BuildPdf failure above.
    public void PrintReceipt()
    {
        if (_receiptPdfPath == null)
        {
            ErrorMessage = "Không có hóa đơn để in.";
            return;
        }
        try
        {
            Process.Start(new ProcessStartInfo(_receiptPdfPath) { UseShellExecute = true });
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"{nameof(PaymentViewModel)}.{nameof(PrintReceipt)}", ex);
            ErrorMessage = "Không thể mở hóa đơn để in.";
        }
    }
}
