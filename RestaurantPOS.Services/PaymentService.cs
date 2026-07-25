using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IShiftRepository _shiftRepository;

    public PaymentService()
    {
        _paymentRepository = new PaymentRepository();
        _orderRepository = new OrderRepository();
        _shiftRepository = new ShiftRepository();
    }

    // Test seam only — production code always uses the parameterless constructor.
    public PaymentService(IPaymentRepository paymentRepository, IOrderRepository orderRepository, IShiftRepository shiftRepository)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _shiftRepository = shiftRepository;
    }

    public CheckoutResult Checkout(int orderId, int cashierUserId, PaymentMethod method)
    {
        // Business rule: only an Open order can be checked out.
        var order = _orderRepository.GetOrderById(orderId);
        if (order == null || order.Status != OrderStatus.Open) return CheckoutResult.OrderNotOpen;

        // Stamp the cashier's current open shift on the payment, so it can be
        // reconciled later. Cash needs a shift open — it's the only method that
        // touches the physical cash drawer being reconciled. Bank transfer doesn't
        // touch the drawer, so it's still allowed with no shift open.
        var shiftId = _shiftRepository.GetOpenShift(cashierUserId)?.ShiftId;
        if (method == PaymentMethod.Cash && shiftId == null) return CheckoutResult.NoOpenShift;

        return _paymentRepository.CheckoutOrder(orderId, cashierUserId, method, shiftId);
    }

    public List<Payment> GetPaymentsBetween(DateTime from, DateTime to) =>
        _paymentRepository.GetPaymentsBetween(from, to);
}
