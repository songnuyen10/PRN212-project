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
    }

    public bool Confirm()
    {
        var cashierUserId = SessionContext.CurrentUser!.UserId;
        var success = _paymentService.Checkout(OrderId, cashierUserId, SelectedMethod);
        ErrorMessage = success ? string.Empty : "Thanh toán thất bại — đơn hàng có thể đã bị thay đổi bởi người khác.";
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
