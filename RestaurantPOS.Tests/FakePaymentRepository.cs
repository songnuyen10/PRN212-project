using RestaurantPOS.BusinessObjects;
using RestaurantPOS.Repositories;

namespace RestaurantPOS.Tests;

public class FakePaymentRepository : IPaymentRepository
{
    private readonly List<Payment> _payments = new();
    public int? CheckoutOrderWasCalledWithShiftId { get; private set; }
    public bool CheckoutOrderWasCalled { get; private set; }

    public void Seed(Payment payment) => _payments.Add(payment);

    public bool CheckoutOrder(int orderId, int cashierUserId, PaymentMethod method, int? shiftId)
    {
        CheckoutOrderWasCalled = true;
        CheckoutOrderWasCalledWithShiftId = shiftId;
        return true;
    }

    public List<Payment> GetPaymentsByShiftId(int shiftId) => _payments.Where(p => p.ShiftId == shiftId).ToList();

    public List<Payment> GetPaymentsBetween(DateTime from, DateTime to) =>
        _payments.Where(p => p.PaidAt >= from && p.PaidAt <= to).ToList();
}
